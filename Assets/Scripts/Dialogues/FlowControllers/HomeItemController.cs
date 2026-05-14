using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HomeItemController : MonoBehaviour
{
    public CGItem cgPlayer;
    [SerializeField] private Sprite windowDust;
    [SerializeField] private Sprite windowClean;
    [SerializeField] private Sprite windowPuzzleClear;
    [SerializeField] private Sprite underTheBedAlt;
    [SerializeField] private GameObject windowPuzzle;
    [SerializeField] private TextAsset magnifierDialogue;
    [SerializeField] private AudioClip specialTime;


    public Item papertowel;
    public Item notes;
    public Item window;
    public Item magnifier;
    public Item under_the_bed;

    private const string PAPERTOWEL = "PaperTowel";
    private const string NOTES = "Notes";
    private const string WINDOW = "window";
    private const string WINDOW_DUST = "window_dust";
    private const string WINDOW_CLEAN = "window_clean";
    private const string MAGNIFIER = "Magnifier";
    private const string UNDERTHEBED = "Under_the_bed";

    private const string USE_TAG = "can_use";

    private List<IItemUseRule> rules = new List<IItemUseRule>();
    private AudioSource _aud;

    private void OnEnable()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnItemTagChanged += HandleItemTagChanged;
            DialogueManager.Instance.OnVariableChanged += HandleVariableChanged;
        }
        if (windowPuzzle != null)
        {
            FocusPuzzle focusPuzzle = windowPuzzle.GetComponent<FocusPuzzle>();
            focusPuzzle.OnPuzzleCompleted += HandlePuzzleCompleted;
        }
    }

    private void OnDisable()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnItemTagChanged -= HandleItemTagChanged;
            DialogueManager.Instance.OnVariableChanged -= HandleVariableChanged;
        }
        if (windowPuzzle != null)
        {
            FocusPuzzle focusPuzzle = windowPuzzle.GetComponent<FocusPuzzle>();
            focusPuzzle.OnPuzzleCompleted -= HandlePuzzleCompleted;
        }
    }

    private void Awake()
    {
        rules.Add(new PaperTowelOnWindowRule());
        window.item_Sprite = windowDust;
    }

    private void Start()
    {
        _aud = GetComponent<AudioSource>();
;    }

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
            case (MAGNIFIER):
                window.item_Sprite = windowPuzzleClear;
                InventoryManager.Instance.QueueItem(magnifier);
                break;
            case (UNDERTHEBED):
                _aud.Pause();
                _aud.clip = specialTime;
                under_the_bed.item_Sprite = underTheBedAlt;
                cgPlayer.CGDisplayInDialogue(under_the_bed);
                _aud.Play();
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
        else if (name == USE_TAG && value)
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

        InventoryManager.Instance.CloseInventory();
        windowPuzzle.SetActive(true);
        GameManager.instance.PushState(GameStateType.Puzzle);
    }

    private void HandlePuzzleCompleted()
    {
        DialogueManager.Instance.NewStory(magnifierDialogue);

        DialogueManager.Instance.OnDialogueStatusChanged += HandlePuzzleDialogueFinished;
    }

    private void HandlePuzzleDialogueFinished(bool isPlaying)
    {
        if (isPlaying)
            return;

        DialogueManager.Instance.OnDialogueStatusChanged -= HandlePuzzleDialogueFinished;

        windowPuzzle.SetActive(false);
        GameManager.instance.PopState(GameStateType.Puzzle);
    }

    public void ExitPuzzle()
    {
        windowPuzzle.SetActive(false);
        GameManager.instance.PopState(GameStateType.Puzzle);
        cgPlayer.ClearDisplay();
    }
}
