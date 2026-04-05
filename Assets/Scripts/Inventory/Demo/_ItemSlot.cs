using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class _ItemSlot : MonoBehaviour, IPointerClickHandler
{
    //======ITEM DATA======//
    public string _itemName;
    public Sprite _itemSprite;
    public bool _isFull;

    //======ITEM SLOT======//
    [SerializeField]
    private Image _itemImage;

    public GameObject _selectedShader;
    public bool _isSelected;

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
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    public void OnLeftClick()
    {
        _inventoryManager.DeselectAllSlots();
        _selectedShader.SetActive(true);
        _isSelected = true;
    }

    public void OnRightClick()
    {

    }


}
