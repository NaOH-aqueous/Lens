using UnityEngine;
using UnityEngine.UI;

public class CGItem : MonoBehaviour
{
    private string itemTag;
    private Image itemImage;
    private GameStateType lastState;

    private void Start()
    {
        itemImage = GetComponent<Image>();
        itemImage.enabled = false;
    }
    public void DisplayItemInfo(Item new_Item)
    {
        itemTag = DialogueManager.Instance.GetItemTag();
        if (GameManager.instance.GetGameStatus() != GameStateType.ItemDisplay)
        {
            lastState = GameManager.instance.GetGameStatus();
        }
        if (!string.IsNullOrEmpty(itemTag) && new_Item.item_Name == itemTag)
        {
            itemImage.enabled = true;
            itemImage.sprite = new_Item.item_Sprite;
            itemImage.SetNativeSize();
            GameManager.instance.ChangeToItemDisplay();
        }
    }

    public void ClearDisplay()
    {
        itemImage.sprite = null;
        itemImage.enabled = false;

        if (lastState == GameStateType.Playing)
        {
            GameManager.instance.ChangeToPlaying();
        }
        else if (lastState == GameStateType.Paused)
        {
            GameManager.instance.ChangeToPaused();
        }
        else if (lastState == GameStateType.Inventory)
        {
            GameManager.instance.ChangeToInventory();
        }
    }
}
