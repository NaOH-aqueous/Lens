using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryMenu;
    public Animator animator;
    public ItemSlot[] slots;

    private void Start()
    {
        InventoryManager.Instance.AssignUI(this);
    }
}
