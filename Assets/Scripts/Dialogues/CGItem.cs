using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

    private void Update()
    {
        if(GameManager.instance.CurrentState != GameStateType.ItemDisplay)
        {
            return;
        }
        if (!PlayItemDialogue)
        {
            return;
        }

        if (InputManager.Instance.IsSubmitPressed())
        {
            if (!DialogueManager.Instance.CheckDialoguePlaying())
            {
                ItemInfoDialogue();
            }
        }
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
            itemDisplayer.SetNativeSize();
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
            Debug.Log("no item dialogue");
        }
    }

    private IEnumerator ClearDisplayCoroutine()
    {
        yield return StartCoroutine(Fade(1f, 0f, 0.25f));

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

        InputManager.Instance.RegisterInteractPressed();
        InputManager.Instance.RegisterSubmitPressed();
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
        itemViewer.SetNativeSize();
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
        yield return StartCoroutine(Fade(1f, 0f, 0.25f));

        Image itemViewer = itemInspector.GetComponentInChildren<Image>();
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
        yield return StartCoroutine(Fade(1f, 0f, 0.25f));

        Image itemViewer = itemInspector.GetComponentInChildren<Image>();
        itemViewer.sprite = null;

        itemInspector.SetActive(false);
        tip.gameObject.SetActive(false);
        InputRouter.Instance.Clear();

        RestorePreviousDisplay();
    }

    public void CGDisplay(Item DisplayItem)
    {
        PlayItemDialogue = false;
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

        PlayItemDialogue = true;

        currentMode = DisplayMode.Inspect;
        InputRouter.Instance.PushLayer(InputLayer.InventoryBlock);

        itemDisplayer.gameObject.SetActive(false);
        CGDisplayer.gameObject.SetActive(false);

        itemInspector.gameObject.SetActive(true);
        itemViewer.sprite = item.item_Sprite;
        itemViewer.SetNativeSize();

        tip.gameObject.SetActive(true);

        EnterDisplayMode();
    }

    public void PlayTooltip()
    {
        tip.gameObject.SetActive(true);
    }

    private IEnumerator Fade(float start, float end, float duration)
    {
        float elapsed = 0f;

        cgCanvasGroup.alpha = start;
        cgCanvasGroup.blocksRaycasts = true;
        cgCanvasGroup.interactable = false;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            cgCanvasGroup.alpha = Mathf.Lerp(start, end, elapsed / duration);

            yield return null;
        }

        cgCanvasGroup.alpha = end;

        cgCanvasGroup.blocksRaycasts = end > 0f;
    }

    private void RestorePreviousDisplay()
    {
        switch (previousMode)
        {
            case DisplayMode.CG:

                CGDisplayer.gameObject.SetActive(true);
                CGDisplayer.sprite = previousItem.item_Sprite;
                tip.gameObject.SetActive(true);

                currentMode = DisplayMode.CG;

                StartCoroutine(Fade(0f, 1f, 0.25f));
                break;

            case DisplayMode.Item:

                itemDisplayer.gameObject.SetActive(true);
                itemDisplayer.sprite = previousItem.item_Sprite;
                tip.gameObject.SetActive(true);

                currentMode = DisplayMode.Item;

                StartCoroutine(Fade(0f, 1f, 0.25f));
                break;

            default:
                ExitDisplayMode();
                break;
        }
    }


    private void EnterDisplayMode()
    {
         StartCoroutine(Fade(0f, 1f, 0.25f));
         GameManager.instance.PushState(GameStateType.ItemDisplay);

    }

    private void ExitDisplayMode()
    {
        InputManager.Instance.RegisterInteractPressed();

        InputManager.Instance.RegisterSubmitPressed();

        GameManager.instance.PopState(
            GameStateType.ItemDisplay
        );

        currentMode = DisplayMode.None;
    }
}
