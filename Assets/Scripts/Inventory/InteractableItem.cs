using UnityEngine;

public class InteractableItem : MonoBehaviour
{

    [SerializeField] private int itemID; // Unique identifier for the item
    [SerializeField] private string itemName;
    [SerializeField] private Texture2D icon;

    Item newItem = new Item();
    private bool playerInRange = false;

    void Start()
    {
        //add this item to the inventory database for later retrieval
        newItem.itemID = itemID;
        newItem.itemName = itemName;
        InventoryManager.Instance.AddItemToDatabase(newItem);
        
    }

    private void Update()
    {
        //pick up if player is nearby
        if (playerInRange)
        {
            PickUpItem();
        }

        //if the picked up item by player is equal to this one, destroy this Gameobject
        if (InventoryManager.Instance.GetPickedUpItem() == newItem)
        {
            Destroy(this.gameObject);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        //set the current item selected to be THIS if player entered
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                InventoryManager.Instance.SetCurrentItem(newItem);
            }

        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            PlayerController player = other.GetComponent<PlayerController>();

            // Check if the player is currently holding this item
            if (player != null && InventoryManager.Instance.GetCurrentItem() == newItem)
            {
                InventoryManager.Instance.ClearCurrentItem();
            }
        }
    }

    private void PickUpItem()
    {
        //pick up the item upon user input
        if (InputManager.Instance.IsInteractPressed())
        {
            Item currentItem = InventoryManager.Instance.GetCurrentItem();
            //pick it up if it exists
            if (currentItem != null)
            {
                InventoryManager.Instance.AddItem(currentItem, currentItem.itemID);
            }
            //_itemDatabase.AddItem(0, this);
        }
    }
}
