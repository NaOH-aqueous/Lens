using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class _ItemSlot : MonoBehaviour
{
    public Item item = new Item();
    [SerializeField] private Sprite _itemIcon;
    [SerializeField] private bool _isFirst;

    //item data(can only be *get* by other scripts)
    public bool IsFull => _isFull;

    private bool _isFull;
    private SpriteRenderer _spriteRenderer;
    private InventoryManager _inventoryManager;

    private void Start()
    {
        _inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
        if (_isFirst)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }

        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public void _AddItem(string itemName, Sprite itemSprite)
    {
        item.item_Name = itemName;
        item.item_Sprite = itemSprite;
        _isFull = true;

        _spriteRenderer.sprite = itemSprite;
        Debug.Log("Slot adding: " + itemName + "\nSprite: " + itemSprite);
    }

    public void ClearSlot()
    {
        item.item_Sprite = null;
        item.item_Name = null;
        _isFull = false;
    }
}
