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

    private Coroutine clearDisplayCoroutine;
    private Coroutine clearInspectCoroutine;
    private Coroutine clearItemCoroutine;
    private Item displayItem;
    private CanvasGroup cgCanvasGroup;
    private void Start()
    {
        itemDisplayer.gameObject.SetActive(false);
        itemInspector.gameObject.SetActive(false);
        CGDisplayer.gameObject.SetActive(false);

        cgCanvasGroup = GetComponent<CanvasGroup>();
    }

    public void DisplayItemInfo(Item new_Item)
    {
        if (!string.IsNullOrEmpty(new_Item.item_Name))
        {
            StartCoroutine(Fade(0f, 1f, 0.25f));
            itemDisplayer.gameObject.SetActive(true);
            itemDisplayer.sprite = new_Item.item_Sprite;
            itemDisplayer.SetNativeSize();
            displayItem = new_Item;
            GameManager.instance.PushState(GameStateType.ItemDisplay);
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

        InputManager.Instance.RegisterInteractPressed();
        InputManager.Instance.RegisterSubmitPressed();

        yield return null;

        GameManager.instance.PopState(GameStateType.ItemDisplay);

        clearDisplayCoroutine = null;
    }

    public void ClearItemOnly()
    {
        if (clearItemCoroutine == null)
        {
            clearItemCoroutine = StartCoroutine(ClearItemOnlyCoroutine());
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
        if (clearDisplayCoroutine == null)
        {
            clearDisplayCoroutine = StartCoroutine(ClearDisplayCoroutine());
        }
    }

    public void InspectItem(Item inspect_Item)
    {
        StartCoroutine(Fade(0f, 1f, 0.25f));
        itemInspector.gameObject.SetActive(true);

        Image itemViewer = itemInspector.GetComponentInChildren<Image>();
        itemViewer.sprite = inspect_Item.item_Sprite;
        itemViewer.SetNativeSize();

        GameManager.instance.PushState(GameStateType.ItemDisplay);
    }

    public void ClearInspect()
    {
        if(clearInspectCoroutine == null)
        {
            clearInspectCoroutine = StartCoroutine(ClearInspectCoroutine());
        }
    }

    private IEnumerator ClearInspectCoroutine()
    {
        yield return StartCoroutine(Fade(1f, 0f, 0.25f));

        Image itemViewer = itemInspector.GetComponentInChildren<Image>();
        itemViewer.sprite = null;
        itemInspector.SetActive(false);
        InputManager.Instance.RegisterInteractPressed();
        InputManager.Instance.RegisterSubmitPressed();

        yield return null;

        GameManager.instance.PopState(GameStateType.ItemDisplay);
        clearInspectCoroutine = null;
    }

    public void CGDisplay(Item DisplayItem)
    {
        if (DisplayItem.item_Sprite != null)
        {
            if (!CGDisplayer.gameObject.activeSelf)
            {
                StartCoroutine(Fade(0f, 1f, 0.25f));
                CGDisplayer.gameObject.SetActive(true);
                GameManager.instance.PushState(GameStateType.ItemDisplay);
            }
            CGDisplayer.sprite = DisplayItem.item_Sprite;
            displayItem = DisplayItem;
        }
    }

    public Item GetCurrentDisplay()
    {
        if (CGDisplayer.isActiveAndEnabled)
        {
            return displayItem;
        }
        return null;
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

}
