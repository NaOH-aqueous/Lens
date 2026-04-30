using UnityEngine;
using UnityEngine.UI;

public class CGItem : MonoBehaviour
{
    public Image itemDisplay;
    public Image itemInspector;

    private GameStateType lastState;

    private void Start()
    {
        itemDisplay.gameObject.SetActive(false);
        itemInspector.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (itemInspector != null && itemInspector.gameObject.activeSelf)
        {
            if (InputManager.Instance != null && InputManager.Instance.IsSubmitPressed())
            {
                ClearInspect();
            }
        }
    }

    public void DisplayItemInfo(Item new_Item)
    {
        if (GameManager.instance.GetGameStatus() != GameStateType.ItemDisplay)
        {
            lastState = GameManager.instance.GetGameStatus();
        }
        if (!string.IsNullOrEmpty(new_Item.item_Name))
        {
            itemDisplay.gameObject.SetActive(true);
            itemDisplay.sprite = new_Item.item_Sprite;
            itemDisplay.SetNativeSize();
            GameManager.instance.ChangeToItemDisplay();
        }
    }

    public void ClearDisplay()
    {
        itemDisplay.sprite = null;
        itemDisplay.gameObject.SetActive(false);

        if (lastState == GameStateType.Playing)
        {
            GameManager.instance.ChangeToPlaying();
        }
        else if (lastState == GameStateType.Paused)
        {
            GameManager.instance.ChangeToPaused();
        }
    }

    public void InspectItem(Item inspect_Item)
    {
        if (inspect_Item.item_Sprite == null)
            return;
        if (itemInspector.sprite == inspect_Item.item_Sprite)
        {
            return;
        }

        if (GameManager.instance.GetGameStatus() != GameStateType.ItemDisplay)
        {
            lastState = GameManager.instance.GetGameStatus();
        }

        itemInspector.gameObject.SetActive(true);
        itemInspector.sprite = inspect_Item.item_Sprite;
        itemInspector.SetNativeSize();
        GameManager.instance.ChangeToItemDisplay();
    }

    public void ClearInspect()
    {
        itemInspector.sprite = null;
        itemInspector.gameObject.SetActive(false);

        if (lastState == GameStateType.Playing)
        {
            GameManager.instance.ChangeToPlaying();
        }
        else if (lastState == GameStateType.Paused)
        {
            GameManager.instance.ChangeToPaused();
        }
    }
}
