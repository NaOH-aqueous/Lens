using UnityEngine;

public class InteractableItem : MonoBehaviour
{
    
    public int itemID; // Unique identifier for the item

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                player.SetCurrentItem(this.gameObject);
            }

        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            // Check if the player is currently holding this item
            if (player != null && player.GetCurrentItem() == this.gameObject)
            {
                player.ClearCurrentItem();
            }
        }
    }
}
