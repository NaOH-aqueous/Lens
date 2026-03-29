using UnityEngine;

public class InteractableItem : MonoBehaviour
{

    [SerializeField] private int itemID; // Unique identifier for the item
    [SerializeField] private string itemName;
    [SerializeField] private Texture2D icon;

    Item newItem = new Item();

    void Start()
    {
        newItem.itemID = itemID;
        newItem.itemName = itemName;

        Inventory.Instance.AddItemToDatabase(newItem);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                Inventory.Instance.SetCurrentItem(newItem);
            }

        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            // Check if the player is currently holding this item
            if (player != null && Inventory.Instance.GetCurrentItem() == newItem)
            {
                Inventory.Instance.ClearCurrentItem();
            }
        }
    }


}
