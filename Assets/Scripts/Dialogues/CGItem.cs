using System.Collections;
using System.Linq.Expressions;
using Unity.VisualScripting;
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
    private Button returnButton;
    private Item displayItem;
    private Item nullItem;

    private void Start()
    {
        itemDisplayer.gameObject.SetActive(false);
        itemInspector.gameObject.SetActive(false);
        CGDisplayer.gameObject.SetActive(false);

        GameManager.instance.OnGameStateChanged += HandleCGStateChange;
    }

    public void DisplayItemInfo(Item new_Item)
    {
        if (!string.IsNullOrEmpty(new_Item.item_Name))
        {
            itemDisplayer.gameObject.SetActive(true);
            itemDisplayer.sprite = new_Item.item_Sprite;
            itemDisplayer.SetNativeSize();
            displayItem = new_Item;
            GameManager.instance.PushState(GameStateType.ItemDisplay);
        }
    }

    private IEnumerator ClearDisplayCoroutine()
    {
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

    public void ClearDisplay()
    {
        if (clearDisplayCoroutine == null)
        {
            clearDisplayCoroutine = StartCoroutine(ClearDisplayCoroutine());
        }
    }

    public void InspectItem(Item inspect_Item)
    {
        itemInspector.gameObject.SetActive(true);

        Image itemViewer = itemInspector.GetComponentInChildren<Image>();
        itemViewer.sprite = inspect_Item.item_Sprite;
        itemViewer.SetNativeSize();

        returnButton = itemInspector.GetComponentInChildren<Button>();
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
            returnButton = CGDisplayer.gameObject.GetComponentInChildren<Button>();
            if (!CGDisplayer.gameObject.activeSelf)
            {
                CGDisplayer.gameObject.SetActive(true);
                GameManager.instance.PushState(GameStateType.ItemDisplay);
            }
            CGDisplayer.sprite = DisplayItem.item_Sprite;
            displayItem = DisplayItem;
        }
    }

    public void CGDisplayInDialogue(Item DisplayItem)
    {
        if (DisplayItem.item_Sprite != null)
        {
            returnButton = null;
            if (!CGDisplayer.gameObject.activeSelf)
            {
                CGDisplayer.gameObject.SetActive(true);
                GameManager.instance.PushState(GameStateType.ItemDisplay);
            }
            CGDisplayer.sprite = DisplayItem.item_Sprite;
            displayItem = DisplayItem;
        }
    }

    public void HandleCGStateChange(GameStateType gameState)
    {
        if(returnButton == null)
        {
            return;
        }
        if(gameState == GameStateType.ItemDisplay)
        {
            if (itemDisplayer.isActiveAndEnabled || CGDisplayer.isActiveAndEnabled)
            {
                EventSystem.current.SetSelectedGameObject(returnButton.gameObject);
            }
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


}
