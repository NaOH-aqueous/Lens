using System.Collections;
using UnityEngine;
using static UnityEditorInternal.VersionControl.ListControl;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [SerializeField] private GameObject InventoryMenu;
    [SerializeField] private _ItemSlot[] itemSlot;

    private bool isInventoryOpen;
    private Animator _anim;
    private bool isTransitioning = false;

    private void Awake()
    {
        //make it a singleton gameobject
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        _anim = InventoryMenu.gameObject.GetComponent<Animator>();
        InventoryMenu.SetActive(false);
        isInventoryOpen = false;

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
        if (InputManager.Instance.isInventoryPressed() && !isTransitioning)
        {
            StartCoroutine(ToggleInventory());
        }

    }

    private IEnumerator ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        isTransitioning = true;
        
        if (isInventoryOpen)
        {
            GameManager.instance.ChangeToInventory();
            InventoryMenu.SetActive(true);
            _anim.SetTrigger("Intro");
        }
        else
        {
            _anim.SetTrigger("Outro");
            GameManager.instance.ChangeToPlaying();
            while (!_anim.GetCurrentAnimatorStateInfo(0).IsName("Outro"))
                yield return null;
            InventoryMenu.SetActive(false);
        }

        isTransitioning = false;
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

    public void UseItem(_ItemSlot slot)
    {

        if (slot == null || !slot.IsFull)
        {
            return;
        }
        Debug.Log("Used item: " + slot.item.item_Name);

        slot.ClearSlot();
    }

    public bool GetInventoryDisplayed()
    {
        return isInventoryOpen;
    }
}
