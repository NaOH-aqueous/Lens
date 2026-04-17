using UnityEngine;
using UnityEngine.InputSystem;

public class _InventoryManager : MonoBehaviour
{

    [SerializeField] private GameObject InventoryMenu;
    [SerializeField] private _ItemSlot[] itemSlot;

    private bool isInventoryOpen;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("ItemSlot array length: " + itemSlot.Length);
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i] == null)
            {
                Debug.LogError("ItemSlot at index " + i + " is not assigned in the inspector.");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleInventory();
        }

    }

    private void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        InventoryMenu.SetActive(isInventoryOpen);
        
        if (isInventoryOpen)
        {
            Time.timeScale = 0f; // Pause the game
            InputSystem.EnableDevice(Mouse.current);
            isInventoryOpen = true;
        }
        else
        {
            Time.timeScale = 1f; // Resume the game
            InputSystem.DisableDevice(Mouse.current);
            isInventoryOpen = false;
        }
    }

    public void _AddItem(string itemName, Sprite itemSprite)
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (!itemSlot[i].IsFull)
            {
                itemSlot[i]._AddItem(itemName, itemSprite);
                return;
            }
        }

        Debug.Log("Trying to add item: " + itemName);

        for (int i = 0; i < itemSlot.Length; i++)
        {
            if(!itemSlot[i].IsFull)
            {
                Debug.Log("Adding to slot: " + i);
                itemSlot[i]._AddItem(itemName, itemSprite);
                return;
            }
        }
        Debug.LogWarning("Inventory is full! Cannot add item: " + itemName);
    }

    public void DeselectAllSlots()
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            itemSlot[i].SetSelected(false);
        }
    }

    public void UseItem(_ItemSlot slot)
    {

        if (slot == null || !slot.IsFull)
        {
            return;
        }
        Debug.Log("Used item: " + slot.ItemName);

        slot.ClearSlot();
    }

}
