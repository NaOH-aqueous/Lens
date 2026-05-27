using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CGItem : MonoBehaviour
{
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
        if (!PlayItemDialogue)
        {
            return;
        }

        if (InputManager.Instance.IsSubmitPressed())
        {
            ItemInfoDialogue();
        }
    }

    public void DisplayItemInfo(Item new_Item)
    {
        if (!string.IsNullOrEmpty(new_Item.item_Name))
        {
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
        StartCoroutine(ClearItemOnlyCoroutine());
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
        StartCoroutine(ClearDisplayCoroutine());
    }

    public void InspectItem(Item inspect_Item)
    {
        itemInspector.gameObject.SetActive(true);
        InputRouter.Instance.PushLayer(InputLayer.InventoryBlock);

        itemViewer.sprite = inspect_Item.item_Sprite;
        itemViewer.SetNativeSize();
        tip.gameObject.SetActive(true);

        EnterDisplayMode();
    }

    public void ClearInspect()
    {
        StartCoroutine(ClearInspectCoroutine());
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

    public void CGDisplay(Item DisplayItem)
    {
        if (DisplayItem.item_Sprite != null)
        {
            if (!CGDisplayer.gameObject.activeSelf)
            {
                CGDisplayer.gameObject.SetActive(true);
                EnterDisplayMode();
            }
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
        if (itemInspector.activeSelf)
        {
            ClearInspect();
        }
        else
        {
            ClearDisplay();
        }

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

    private void EnterDisplayMode()
    {
        StartCoroutine(Fade(0f, 1f, 0.25f));

        GameManager.instance.PushState(
            GameStateType.ItemDisplay
        );
    }

    private void ExitDisplayMode()
    {
        InputManager.Instance.RegisterInteractPressed();

        InputManager.Instance.RegisterSubmitPressed();

        GameManager.instance.PopState(
            GameStateType.ItemDisplay
        );
    }
}
