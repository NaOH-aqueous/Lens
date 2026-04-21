using UnityEngine;
using UnityEngine.UIElements;

public class CGItem : MonoBehaviour
{
    private string itemTag;
    private Image itemImage;

    private void Start()
    {
        itemImage = GetComponent<Image>();
    }
    public void DisplayItemInfo(string itemName, Sprite itemSprite)
    {
        itemTag = DialogueManager.Instance.GetItemTag();
        if (!string.IsNullOrEmpty(itemTag) && itemName == itemTag)
        {
            itemImage.sprite = itemSprite;
        }
    }
}
