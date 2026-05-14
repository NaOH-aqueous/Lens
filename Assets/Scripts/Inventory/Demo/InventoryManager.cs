using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [SerializeField] private GameObject InventoryMenu;
    [SerializeField] private ItemSlot[] itemSlot;

    private HashSet<string> itemNames = new HashSet<string>();

    private bool isInventoryOpen;
    private Animator _anim;
    private bool isTransitioning = false;
    private ItemSlot selectedSlot;
    private Queue<Item> itemQueue = new Queue<Item>();
    private bool isProcessingQueue = false;

    private Item currentItem;

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

        //return when there's not itemslots assigned
        if (itemSlot == null || itemSlot.Length == 0)
        {
            Debug.LogWarning("No item slots assigned to InventoryManager.");
            return;
        }

        for (int i = 0, len = itemSlot.Length; i < len; i++)
        {
            if (i == 0)
            {
                //make the first slot selected by default
                selectedSlot = itemSlot[0];
            }
            if (itemSlot[i] == null)
            {
                //if the itemslot is created but not assigned, send error to console
                Debug.LogError("ItemSlot at index " + i + " is not assigned in the inspector.");
            }
        }
    }

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
            GameManager.instance.PushState(GameStateType.Inventory);
            InventoryMenu.SetActive(true);
            _anim.SetTrigger("Intro");

            StartCoroutine(SelectFirstSlotNextFrame());
        }
        else
        {
            _anim.SetTrigger("Outro");
            InputManager.Instance.RegisterInteractPressed();
            GameManager.instance.PopState(GameStateType.Inventory);
            while (!_anim.GetCurrentAnimatorStateInfo(0).IsName("Outro"))
                yield return null;
            InventoryMenu.SetActive(false);
        }

        isTransitioning = false;
    }

    public void CloseInventory()
    {
        if (isInventoryOpen)
        {
            StartCoroutine(ToggleInventory());
        }
    }

    public IEnumerator SelectFirstSlotNextFrame()
    {
        yield return new WaitForEndOfFrame();
        yield return null; 
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(selectedSlot.gameObject);
    }

    private void AddItem(Item new_Item)
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (!itemSlot[i].IsFull)
            {
                Debug.Log("Adding to slot: " + i);
                itemSlot[i]._AddItemToSlot(new_Item);
                selectedSlot = itemSlot[i];
                itemNames.Add(new_Item.item_Name);
                return;
            }
        }
        Debug.LogWarning("Inventory is full! Cannot add item: " + new_Item.item_Name);
    }

    public void QueueItem(Item item)
    {
        itemQueue.Enqueue(item);
        if (!isProcessingQueue)
        {
            StartCoroutine(ProcessQueue());
        }
    }

    private IEnumerator ProcessQueue()
    {
        isProcessingQueue = true;
        yield return null;
        yield return new WaitForEndOfFrame();

        while (itemQueue.Count > 0)
        {
            Item item = itemQueue.Dequeue();
            AddItem(item);
        }

        isProcessingQueue = false;
    }

    public void SetCurrentItem(Item item) 
    { 
        if(item == null)
        {
            return;
        }
        currentItem = item;
    }

    public Item GetCurrentItem()
    {
        return currentItem;
    }

    public void UseItem(Item item)
    {
        if (item == null)
        {
            return;
        }
        if (GetItem(item))
        {
            for(int i = 0; i < itemSlot.Length; i++)
            {
                if (itemSlot[0].item == item)
                {
                    Debug.Log("used item: " + item.item_Name);
                    itemSlot[i].ClearSlot();
                    itemNames.Remove(item.item_Name);
                    currentItem = null;
                }
            }
        }
    }

    private bool GetItem(Item item)
    {
        if (itemNames.Contains(item.item_Name))
        {
            return true;
        }
        return false;
    }

    public Item GetItemByName(string itemName)
    {
        if (itemNames.Contains(itemName))
        {
            for (int i = 0; i < itemSlot.Length; i++)
            {
                if (itemSlot[0].item.item_Name == itemName)
                {
                    return itemSlot[0].item;
                }
            }
        }

        return null;
    }
}
