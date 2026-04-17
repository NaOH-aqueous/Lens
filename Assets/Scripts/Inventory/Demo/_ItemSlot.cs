using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class _ItemSlot : MonoBehaviour, IPointerClickHandler
{
    // 物品数据（只对外提供 getter）或者
    //public string ItemName { get { return _itemName; } }
    //public bool IsFull { get { return _isFull; } }
    public string ItemName => _itemName;
    public bool IsFull => _isFull;

    // This class represents a single slot in the inventory. It can hold an item and display its name and sprite.
    [SerializeField] private Image _itemImage;  // 物品图标
    [SerializeField] private GameObject _selectedShader; // 选中高亮效果
    [SerializeField] private Sprite _emptySprite;  // 空槽位占位图

    // The item name and sprite are stored as private fields, and the slot can be marked as full or empty.
    [SerializeField] private string _itemName;
    [SerializeField] private Sprite _itemSprite;
    [SerializeField] private bool _isSelected;
    private bool _isFull;
    private _InventoryManager _inventoryManager;

    private void Start()
    {
        _inventoryManager = GameObject.Find("UI Canvas").GetComponent<_InventoryManager>();
    }

    public void _AddItem(string itemName, Sprite itemSprite)
    {
        this._itemName = itemName;
        this._itemSprite = itemSprite;
        _isFull = true;

        _itemImage.sprite = _itemSprite;
        _itemImage.enabled = true;

        Debug.Log("Slot adding: " + itemName + "\nSprite: " + itemSprite);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            _inventoryManager.DeselectAllSlots();
            _selectedShader.SetActive(true);
            _isSelected = true;
        }
        else if (eventData.button == PointerEventData.InputButton.Right && _isFull)
        {
            if (eventData.clickCount >= 2)
            {
                _inventoryManager.UseItem(this);
            }
        }
    }

    public void ClearSlot()
    {
        _itemName = null;
        _itemSprite = null;
        _isFull = false;
        _itemImage.sprite = _emptySprite;

        if (_selectedShader != null) 
        {
            _selectedShader.SetActive(false);
        }

        _isSelected = false;
    }

    public void SetSelected(bool selected)
    {
        _isSelected = selected;
        if (_selectedShader != null)
        {
            _selectedShader.SetActive(selected);
        }
    }
}
