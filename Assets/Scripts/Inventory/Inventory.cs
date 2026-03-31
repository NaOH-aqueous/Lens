using Ink.Parsed;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;



public class Inventory : MonoBehaviour
{
    //Make it a singleton object
    public static Inventory Instance{ get; private set; }

    // List to record the items in the inventory
    private List<Item> m_itemDatabase = new List<Item>();

    // Reference to the currently interactable item
    private Item current_item;
    private Item pickedUpItem;

    private PlayerController player;


    private void Awake()
    {
        //make it a singleton gameobject
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        EventSystem.current.SetSelectedGameObject(null);
    }

    private void Start()
    {
        player = GameObject.FindAnyObjectByType<PlayerController>();
    }

    public bool AddItem(Item item, int itemID)
    {
        // Find the item in the database using the item ID
        Item itemData = m_itemDatabase.Find(item => item.itemID == itemID);

        if (itemData != null)
        {
            Debug.Log("Item added to inventory: " + itemData.itemName);
            // Add the item to the player's inventory
            player.inventory.Add(itemData);
            pickedUpItem = itemData;
            return true;
        }
        else
        {
            Debug.Log("Item does not exist");
            return false;
        }

            // check if item matches something in the inventory
            //foreach (var item in itemDatabase)
            //{
            //    if (item.itemID == _itemID)
            //    {
            //        Debug.Log("Item added to inventory: " + item.itemName);
            //        // Add the item to the player's inventory (for simplicity, we add it to the first slot)
            //        //check for available slot in inventory
            //        player.inventory.Add(item); 
            //        return true;
            //    }
            //}
    }

    public void RemoveItem(int _itemID)
    {
        // Remove the item from the inventory
        //check if item matches something in the inventory
        foreach (var item in m_itemDatabase)
        {
            if (item.itemID == _itemID)
            {
                Debug.Log("Item removed from inventory: " + item.itemName);
                // Remove the item from the player's inventory (for simplicity, we clear the first slot)
                player.inventory.Remove(item);
            }
        }

    }
    public Item GetPickedUpItem()
    {
        return pickedUpItem;
    }

    public void AddItemToDatabase(Item item)
    {
        m_itemDatabase.Add(item);
    }

    public void SetCurrentItem(Item item)
    {
        current_item = item;
    }

    public void ClearCurrentItem()
    {
        current_item = null;
    }

    public Item GetCurrentItem()
    {
        return current_item;
    }

    public List<Item> GetInventoryList()
    {
        return m_itemDatabase;
    }

    public void GetInventoryContent()
    {

        Debug.Log("=== Inventory Contents ===");

        if (player.inventory.Count == 0)
        {
            Debug.Log("Inventory is empty.");
        }
        else
        {
            for (int i = 0; i < player.inventory.Count; i++)
            {
                if (player.inventory[i] != null)
                {
                    Debug.Log($"Slot {i}: {player.inventory[i].itemName} (ID: {player.inventory[i].itemID})");
                }
                else
                {
                    Debug.Log($"Slot {i}: Empty");
                }
            }
        }

        Debug.Log("======================");
    }
}

[System.Serializable]
public class Item
{
    public string itemName;
    public int itemID;
}
