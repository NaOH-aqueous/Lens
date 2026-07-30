using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;

public class CGItem : MonoBehaviour
{
    private enum DisplayMode
    {
        None,
        CG,
        Item,
        Inspect
    }

    public Image itemDisplayer;
    public GameObject itemInspector;
    public Image CGDisplayer;
    public Image tip;
    public bool PlayItemDialogue { set; get; }

    private Coroutine clearDisplayCoroutine;
    private Coroutine clearInspectCoroutine;
    private Coroutine clearItemCoroutine;
    private Item displayItem;
    private CanvasGroup cgCanvasGroup;
    private Image itemViewer;
    private DisplayMode currentMode = DisplayMode.None;
    private DisplayMode previousMode = DisplayMode.None;

    private Item previousItem;

    // Track whether we've subscribed to the submit event to avoid double subscriptions
    private bool submitSubscribed = false;

    private void Start()
    {
        itemViewer =
            itemInspector.GetComponentInChildren<Image>();

        cgCanvasGroup =
            GetComponent<CanvasGroup>();

        itemDisplayer.gameObject.SetActive(false);
        itemInspector.SetActive(false);
        CGDisplayer.gameObject.SetActive(false);
        tip.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        UnsubscribeSubmit();
    }

    public void DisplayItemInfo(Item new_Item)
    {
        PlayItemDialogue = false;
        if (!string.IsNullOrEmpty(new_Item.item_Name))
        {
            currentMode = DisplayMode.Item;
            //display item in the player
            itemDisplayer.gameObject.SetActive(true);
            itemDisplayer.sprite = new_Item.item_Sprite;
            displayItem = new_Item;

            if (!CGDisplayer.isActiveAndEnabled)
            {
                EnterDisplayMode();
            }
        }
    }

    private void ItemInfoDialogue()
    {
        if(displayItem == null)
        {
            return;
        }

        if(displayItem.item_Dialogue == null)
        {
            return;
        }
        else
        {
            DialogueManager.Instance.NewStory(displayItem.item_Dialogue);
        }
    }

    private IEnumerator ClearDisplayCoroutine()
    {
        // fade out using TweenHelper (DOTween) with default easing, unscaled time
        if(cgCanvasGroup != null)
        {
            cgCanvasGroup.alpha = 1f;
            var fadeOut = TweenHelper.FadeCanvasGroup(cgCanvasGroup, 0f, 0.25f, true, DG.Tweening.Ease.OutQuad);
            yield return fadeOut.WaitForCompletion();
        }

        itemDisplayer.sprite = null;
        CGDisplayer.sprite = null;

        displayItem = null;

        itemDisplayer.gameObject.SetActive(false);
        CGDisplayer.gameObject.SetActive(false);
        tip.gameObject.SetActive(false);

        yield return null;

        ExitDisplayMode();

        clearDisplayCoroutine = null;
    }

    public void ClearItemOnly()
    {
        if(clearItemCoroutine == null)
        {
            StartCoroutine(ClearItemOnlyCoroutine());
        }
    }

    private IEnumerator ClearItemOnlyCoroutine()
    {
        itemDisplayer.sprite = null;
        displayItem = null;
        itemDisplayer.gameObject.SetActive(false);

        // legacy flag clearing isn't needed with event-driven input, keep for compatibility if other systems rely on it:
        if (InputManager.Instance != null)
        {
            InputManager.Instance.RegisterInteractPressed();
            InputManager.Instance.RegisterSubmitPressed();
        }

        yield return null;
        clearItemCoroutine = null;
    }

    public void ClearDisplay()
    {
        if(clearDisplayCoroutine == null)
        {
            StartCoroutine(ClearDisplayCoroutine());
        }
    }

    public void InspectItem(Item inspect_Item)
    {
        currentMode = DisplayMode.Inspect;
        itemInspector.gameObject.SetActive(true);
        PlayItemDialogue = true;
        displayItem = inspect_Item;
        InputRouter.Instance.PushLayer(InputLayer.InventoryBlock);

        itemViewer.sprite = inspect_Item.item_Sprite;
        tip.gameObject.SetActive(true);

        EnterDisplayMode();
    }

    public void ClearInspect()
    {
        if(clearInspectCoroutine == null)
        {
            StartCoroutine(ClearInspectCoroutine());
        }
    }

    private IEnumerator ClearInspectCoroutine()
    {
        // fade out using TweenHelper (DOTween) with default easing, unscaled time

        if(cgCanvasGroup != null)
        {
            cgCanvasGroup.alpha = 1f;
            var fadeOut = TweenHelper.FadeCanvasGroup(cgCanvasGroup, 0f, 0.25f, true, DG.Tweening.Ease.OutQuad);
            yield return fadeOut.WaitForCompletion();
        }

        itemViewer.sprite = null;
        itemInspector.SetActive(false);
        tip.gameObject.SetActive(false);

        yield return null;

        ExitDisplayMode();
        InputRouter.Instance.Clear();
        clearInspectCoroutine = null;
    }

    public void ExitInspect() //exiting without clearing everything
    {
        StartCoroutine(ExitInspectCoroutine());
    }

    private IEnumerator ExitInspectCoroutine()
    {
        // fade out using TweenHelper (DOTween) with default easing, unscaled time
        cgCanvasGroup.alpha = 1f;
        var fadeOut = TweenHelper.FadeCanvasGroup(cgCanvasGroup, 0f, 0.25f, true, DG.Tweening.Ease.OutQuad);
        yield return fadeOut.WaitForCompletion();

        itemViewer.sprite = null;

        itemInspector.SetActive(false);
        tip.gameObject.SetActive(false);
        InputRouter.Instance.Clear();

        RestorePreviousDisplay();
    }

    public void CGDisplay(Item DisplayItem)
    {
        PlayItemDialogue = true;
        if (DisplayItem.item_Sprite != null)
        {
            if (!CGDisplayer.gameObject.activeSelf)
            {
                CGDisplayer.gameObject.SetActive(true);
                EnterDisplayMode();
            }
            currentMode = DisplayMode.CG;
            CGDisplayer.sprite = DisplayItem.item_Sprite;
            displayItem = DisplayItem;
        }
    }

    public Item GetCurrentDisplay()
    {
        return displayItem;
    }

    public void ExitCGMode()
    {
        if (currentMode == DisplayMode.Inspect)
        {
            ExitInspect();
        }
        else
        {
            ClearDisplay();
        }
    }

    public void TransitionToInpsectMode(Item item) //transition from other mode to inspect
    {
        previousMode = currentMode;
        previousItem = displayItem;
        displayItem = item;

        PlayItemDialogue = true;

        currentMode = DisplayMode.Inspect;
        InputRouter.Instance.PushLayer(InputLayer.InventoryBlock);

        itemDisplayer.gameObject.SetActive(false);
        CGDisplayer.gameObject.SetActive(false);

        itemInspector.gameObject.SetActive(true);
        itemViewer.sprite = item.item_Sprite;

        tip.gameObject.SetActive(true);

        EnterDisplayMode();
    }

    public void PlayTooltip()
    {
        tip.gameObject.SetActive(true);
        PlayItemDialogue = true;
    }

    private void RestorePreviousDisplay()
    {
        switch (previousMode)
        {
            case DisplayMode.CG:
                displayItem = previousItem;
                CGDisplayer.gameObject.SetActive(true);
                CGDisplayer.sprite = previousItem.item_Sprite;
                tip.gameObject.SetActive(true);

                currentMode = DisplayMode.CG;

                // fade in using TweenHelper (DOTween), don't block
                cgCanvasGroup.alpha = 0f;
                TweenHelper.FadeCanvasGroup(cgCanvasGroup, 1f, 0.25f, true, DG.Tweening.Ease.OutQuad);
                break;

            case DisplayMode.Item:
                displayItem = previousItem;
                itemDisplayer.gameObject.SetActive(true);
                itemDisplayer.sprite = previousItem.item_Sprite;
                tip.gameObject.SetActive(true);

                currentMode = DisplayMode.Item;

                // fade in using TweenHelper (DOTween), don't block
                cgCanvasGroup.alpha = 0f;
                TweenHelper.FadeCanvasGroup(cgCanvasGroup, 1f, 0.25f, true, DG.Tweening.Ease.OutQuad);
                break;

            default:
                ExitDisplayMode();
                break;
        }
    }

    private void EnterDisplayMode()
    {
         // fade in using TweenHelper (DOTween), don't block
         cgCanvasGroup.alpha = 0f;
         if (cgCanvasGroup != null)
        {
            TweenHelper.FadeCanvasGroup(cgCanvasGroup, 1f, 0.25f, true, DG.Tweening.Ease.OutQuad);
        }

         GameManager.instance.PushState(GameStateType.ItemDisplay);

         // Subscribe to submit input when entering display mode
         SubscribeSubmit();
    }

    private void ExitDisplayMode()
    {
        previousMode = currentMode = DisplayMode.None;
        displayItem = null;
        currentMode = DisplayMode.None;

        // Unsubscribe from submit input when leaving display mode
        UnsubscribeSubmit();

        if (InputManager.Instance != null)
        {
            InputManager.Instance.RegisterInteractPressed();
            InputManager.Instance.RegisterSubmitPressed();
        }

        GameManager.instance.PopState(GameStateType.ItemDisplay);
    }

    private void SubscribeSubmit()
    {
        if (submitSubscribed) return;
        if (InputManager.Instance == null) return;

        InputManager.Instance.SubmitPerformed -= OnSubmitPerformed;
        InputManager.Instance.SubmitPerformed += OnSubmitPerformed;
        submitSubscribed = true;
    }

    private void UnsubscribeSubmit()
    {
        if (!submitSubscribed) return;
        if (InputManager.Instance == null)
        {
            submitSubscribed = false;
            return;
        }

        InputManager.Instance.SubmitPerformed -= OnSubmitPerformed;
        submitSubscribed = false;
    }

    // Event handler for submit input (replaces polling)
    private void OnSubmitPerformed()
    {
        // Mirror previous guards used in Update
        if (GameManager.instance == null || GameManager.instance.CurrentState != GameStateType.ItemDisplay)
            return;

        if (DialogueManager.Instance != null && DialogueManager.Instance.CheckDialoguePlaying())
            return;

        if (!PlayItemDialogue)
            return;

        ItemInfoDialogue();
    }
}
