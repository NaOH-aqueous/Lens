using UnityEngine;
using UnityEngine.UI;

public class CGItem : MonoBehaviour
{
    private string itemTag;
    private Image itemImage;

    private void Start()
    {
        itemImage = GetComponent<Image>();
        itemImage.enabled = false;
    }
    public void DisplayItemInfo(Item new_Item)
    {
        itemTag = DialogueManager.Instance.GetItemTag();
        if (!string.IsNullOrEmpty(itemTag) && new_Item.item_Name == itemTag)
        {
            itemImage.enabled = true;
            itemImage.sprite = new_Item.item_Sprite;
            itemImage.SetNativeSize();
        }
    }

    public void ClearDisplay()
    {
        itemImage.sprite = null;
        itemImage.enabled = false;
    }
}
