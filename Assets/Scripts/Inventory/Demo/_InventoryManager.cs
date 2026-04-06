using UnityEngine;
using UnityEngine.InputSystem;

public class _InventoryManager : MonoBehaviour
{

    public GameObject InventoryMenu;
    private bool isInventoryOpen;
    public _ItemSlot[] itemSlot;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.M) && isInventoryOpen)
        {
            Time.timeScale = 1f; // Resume the game
            InventoryMenu.SetActive(false);
            isInventoryOpen = false;

            InputSystem.EnableDevice(Mouse.current);
        }
        else if (Input.GetKeyDown(KeyCode.M) && !isInventoryOpen)
        {
            Time.timeScale = 0f; // Pause the game
            InventoryMenu.SetActive(true);
            isInventoryOpen = true;

            InputSystem.EnableDevice(Mouse.current);
        }

    }

    public void _AddItem(string itemName, Sprite itemSprite)
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (!itemSlot[i]._isFull)
            {
                itemSlot[i]._AddItem(itemName, itemSprite);
                return;
            }
        }
    }

    public void DeselectAllSlots()
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            itemSlot[i]._selectedShader.SetActive(false);
            itemSlot[i]._isSelected = false;
        }
    }

    public void UseItem(_ItemSlot slot)
    {

        if (slot == null || !slot._isFull)
        {
            return;
        }
        Debug.Log("Used item: " + slot._itemName);

        slot.ClearSlot();

    }

}
