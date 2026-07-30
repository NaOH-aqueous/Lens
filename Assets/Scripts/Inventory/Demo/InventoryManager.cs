using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    public bool IsReady { get; private set; }

    private GameObject InventoryMenu;
    private readonly List<ItemSlot> itemSlot = new();

    private HashSet<string> itemNames = new HashSet<string>();

    private bool isInventoryOpen;
    private Animator _anim;
    private bool isTransitioning = false;
    private ItemSlot selectedSlot;
    private Queue<Item> itemQueue = new Queue<Item>();
    private bool isProcessingQueue = false;

    private Item currentItem;

    private bool inventorySubscribed = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        IsReady = false;
        //return when there's not itemslots assigned
        if (itemSlot == null || itemSlot.Count == 0)
        {
            Debug.LogWarning("No item slots assigned to InventoryManager.");
            return;
        }

        for (int i = 0, len = itemSlot.Count; i < len; i++)
        {
            if (i == 0)
            {
                // make the first slot selected by default
                selectedSlot = itemSlot[0];
            }
        }

        InventoryMenu.SetActive(false);
        isInventoryOpen = false;

        SubscribeInventory();
    }

    public void AssignUI(InventoryUI ui)
    {
        InventoryMenu = ui.inventoryMenu;
        _anim = ui.animator;

        itemSlot.Clear();
        itemSlot.AddRange(ui.slots);

        Debug.Log($"Manager slot count = {itemSlot.Count}");
        if (itemSlot.Count > 0)
            selectedSlot = itemSlot[0];

        InventoryMenu.SetActive(false);
        Debug.Log("inventory assign");
        IsReady = true;
    }

    private void OnEnable()
    {
        // Try to subscribe when the component becomes enabled
        SubscribeInventory();
    }

    private void OnDisable()
    {
        UnsubscribeInventory();
    }

    private void OnDestroy()
    {
        UnsubscribeInventory();
    }

    private void SubscribeInventory()
    {
        if (inventorySubscribed)
            return;

        if (InputManager.Instance == null)
            return;

        // defensive unsubscribe then subscribe
        InputManager.Instance.InventoryPerformed -= OnInventoryPerformed;
        InputManager.Instance.InventoryPerformed += OnInventoryPerformed;
        inventorySubscribed = true;
    }

    private void UnsubscribeInventory()
    {
        if (!inventorySubscribed)
            return;

        if (InputManager.Instance != null)
            InputManager.Instance.InventoryPerformed -= OnInventoryPerformed;

        inventorySubscribed = false;
    }

    // Event handler replacing the old polling behaviour
    private void OnInventoryPerformed()
    {
        // Apply the same guards that previously existed in Update
        if (DialogueManager.Instance.CheckDialoguePlaying())
        {
            return;
        }

        if (isTransitioning)
        {
            return;
        }

        StartCoroutine(ToggleInventory());
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
            while (!_anim.GetCurrentAnimatorStateInfo(0).IsName("Outro"))
                yield return null;
            EventSystem.current.SetSelectedGameObject(null);
            InventoryMenu.SetActive(false);
            GameManager.instance.PopState(GameStateType.Inventory);
        }

        isTransitioning = false;
    }


    public IEnumerator CloseInventory()
    {
        if (!isInventoryOpen)
            yield break;

        isTransitioning = true;

        yield return ToggleInventory();

        yield return null;

        isTransitioning = false;
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
        for (int i = 0; i < itemSlot.Count; i++)
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
        Debug.Log("Queued " + item.item_Name);
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
        if (item == null)
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
            return;

        for (int i = 0; i < itemSlot.Count; i++)
        {
            if (itemSlot[i].item == item)
            {
                Debug.Log(
                    "used item: "
                    + item.item_Name
                );

                itemSlot[i].ClearSlot();

                itemNames.Remove(
                    item.item_Name
                );

                if (currentItem == item)
                    currentItem = null;

                return;
            }
        }
    }


    public Item GetItemByName(string itemName)
    {
        if (itemNames.Contains(itemName))
        {
            for (int i = 0; i < itemSlot.Count; i++)
            {
                if (itemSlot[i].item != null && itemSlot[i].item.item_Name == itemName)
                {
                    return itemSlot[i].item;
                }
            }
        }

        return null;
    }

    public void RegisterSlot(ItemSlot slot)
    {
        if (!itemSlot.Contains(slot))
        {
            itemSlot.Add(slot);
        }

        if (selectedSlot == null)
            selectedSlot = slot;
    }

    public void UnregisterSlot(ItemSlot slot)
    {
        itemSlot.Remove(slot);

        if (selectedSlot == slot)
            selectedSlot = itemSlot.Count > 0 ? itemSlot[0] : null;
    }
}
