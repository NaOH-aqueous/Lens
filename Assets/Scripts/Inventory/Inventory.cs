using System.Collections.Generic;
using Ink.Parsed;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // List to hold the items in the inventory
    public List<Item> itemDatabase = new List<Item>();
    
    public void AddItem(int _itemID, PlayerController player)
    {
        // check if item matches something in the inventory
        foreach (var item in itemDatabase)
        {
            if (item.itemID == _itemID)
            {
                Debug.Log("Item added to inventory: " + item.itemName);
                // Add the item to the player's inventory (for simplicity, we add it to the first slot)
                //check for available slot in inventory
                player.inventory.Add(item); 
                return;
            }
        }

        Debug.Log("Item does not exist");

    }

    public void RemoveItem(int _itemID, PlayerController player)
    {
        // Remove the item from the inventory
        //check if item matches something in the inventory
        foreach (var item in itemDatabase)
        {
            if (item.itemID == _itemID)
            {
                Debug.Log("Item removed from inventory: " + item.itemName);
                // Remove the item from the player's inventory (for simplicity, we clear the first slot)
                player.inventory.Remove(item);
            }
        }

    }

}
