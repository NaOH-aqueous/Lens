using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HomeItemController : MonoBehaviour
{
    public CGItem cgPlayer;
    [SerializeField] private Sprite windowDust;
    [SerializeField] private Sprite windowClean;

    private const string PAPERTOWEL = "PaperTowel";
    public Item papertowel;

    private const string NOTES = "Notes";
    public Item notes;

    private const string WINDOW = "window";
    private const string WINDOW_DUST = "window_dust";
    private const string WINDOW_CLEAN = "window_clean";
    public Item window;
    private const string USE_TAG = "can_use";

    private List<IItemUseRule> rules = new List<IItemUseRule>();


    private void OnEnable()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnItemTagChanged += HandleItemTagChanged;
            DialogueManager.Instance.OnVariableChanged += HandleVariableChanged;
        }
    }

    private void OnDisable()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnItemTagChanged -= HandleItemTagChanged;
            DialogueManager.Instance.OnVariableChanged -= HandleVariableChanged;
        }
    }

    private void Awake()
    {
        rules.Add(new PaperTowelOnWindowRule());
        window.item_Sprite = windowDust;
    }

    public bool CanUseItem()
    {
        Item inventoryItem = InventoryManager.Instance.GetCurrentItem();
        Item worldItem = cgPlayer.GetCurrentDisplay();

        Debug.Log($"[CanUseItem] inventory={inventoryItem?.item_Name}, world={worldItem?.item_Name}");

        if (inventoryItem == null || worldItem == null)
            return false;

        foreach (var rule in rules)
        {
            bool result = rule.CanUse(inventoryItem, worldItem);
            Debug.Log($"Rule {rule.GetType().Name} = {result}");

            if (result)
                return true;
        }

        return false;
    }

    public void TryUseItem()
    {
        Item inventoryItem = InventoryManager.Instance.GetCurrentItem();
        Item worldItem = cgPlayer.GetCurrentDisplay();

        if (inventoryItem == null || worldItem == null)
            return;

        foreach (var rule in rules)
        {
            if (rule.CanUse(inventoryItem, worldItem))
            {
                ApplyRuleEffect(rule, inventoryItem, worldItem);
                return;
            }
        }
    }

    private void HandleItemTagChanged(string newTag)
    {
        Debug.Log("Item tag changed to: " + newTag);

        //return directly when the string is empty or null
        if (string.IsNullOrEmpty(newTag))
        {
            return;
        }
        switch (newTag)
        {
            case (PAPERTOWEL):
                Debug.Log("display");
                cgPlayer.DisplayItemInfo(papertowel);
                InventoryManager.Instance.QueueItem(papertowel);
                break;
            case (WINDOW_DUST):
                window.item_Sprite = windowDust;
                cgPlayer.CGDisplay(window);
                break;
            case (WINDOW_CLEAN):
                window.item_Sprite = windowClean;
                cgPlayer.CGDisplay(window);
                break;
            case ("clearDisplay"):
                cgPlayer.ClearDisplay();
                break;
        }
    }

    private void HandleVariableChanged(string name, Ink.Runtime.Object value)
    {
        if (name != "read_notes" && name != USE_TAG)
            return;

        if (name == "read_notes" && value)
        {
            cgPlayer.InspectItem(notes);
        }
        else if (name == USE_TAG)
        {
            Debug.Log("can use is" + value);
        }
        else
        {
            cgPlayer.ClearInspect();
        }
    }

    private void ApplyRuleEffect(IItemUseRule rule, Item inventoryItem, Item worldItem)
    {
        if (rule is PaperTowelOnWindowRule)
        {
            SetWindowClean();
        }
    }

    public void SetWindowClean()
    {
        window.item_Sprite = windowClean;
        cgPlayer.CGDisplay(window);
        DialogueManager.Instance.SetBoolVariable("window_clean", true);
    }

}
