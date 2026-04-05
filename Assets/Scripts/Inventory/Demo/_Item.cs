using UnityEngine;

public class _Item : MonoBehaviour
{
    
    [SerializeField]
    private string item_Name;

    [SerializeField]
    private Sprite item_Sprite;

    private _InventoryManager inventoryManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventoryManager = GameObject.Find("UI Canvas").GetComponent<_InventoryManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            inventoryManager._AddItem(item_Name, item_Sprite);
            Destroy(gameObject);
        }
    }

}
