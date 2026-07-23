
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Item item;
    public Image _itemImage;

    public Sprite _default;

    //item data(can only be *get* by other scripts)
    public bool IsFull => _isFull;

    private bool _isFull;

    public void _AddItemToSlot(Item new_Item)
    {
        if (!_isFull)
        {
            item = new_Item;
            _isFull = true;

            if(item.item_Icon != null)
            {
                _itemImage.sprite = item.item_Icon;
                _itemImage.raycastTarget = false;
                _itemImage.maskable = false;
            }
            Debug.Log("Slot adding: " + new_Item.item_Name + "\nSprite: " + new_Item.item_Sprite);
        }
        else
        {
            Debug.Log("This grid is full! cannot add new item");
        }
    }

    public void ClearSlot()
    {
        item = null;
        _itemImage.sprite = _default;
        _isFull = false;
    }

    public void ItemDialogue(TextAsset story)
    {
        if (_isFull)
        {
            InputManager.Instance.RegisterInteractPressed();
            Debug.Log("current item is"+ item.item_Name);
            InventoryManager.Instance.SetCurrentItem(item);
            DialogueManager.Instance.NewStory(story);
        }
    }
}
