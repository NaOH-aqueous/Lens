using UnityEngine;

public class _Item : MonoBehaviour
{
    
    [SerializeField]
    private string item_Name;

    [SerializeField]
    private Sprite item_Sprite;

    private _InventoryManager inventoryManager;

    // This bool is used to check if the player is in range of the item, so that they can pick it up.
    private bool playerInRange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventoryManager = GameObject.Find("UI Canvas").GetComponent<_InventoryManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            inventoryManager._AddItem(item_Name, item_Sprite);
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerInRange = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerInRange = false;
        }
    }
}
