using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Item item = new Item();
    [SerializeField] private bool _isFirst;
    public Image _itemImage;

    //item data(can only be *get* by other scripts)
    public bool IsFull => _isFull;

    private bool _isFull;

    private void Start()
    {
        if (_isFirst)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }

    public void _AddItemToSlot(Item new_Item)
    {
        if (!_isFull)
        {
            item = new_Item;
            _isFull = true;

            if(item.item_Icon != null)
            {
                _itemImage.sprite = item.item_Icon;
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
        item.item_Sprite = null;
        item.item_Name = null;
        item.item_Icon = null;
        _isFull = false;
    }


}
