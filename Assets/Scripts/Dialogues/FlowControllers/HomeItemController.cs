using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HomeItemController : MonoBehaviour, IDialogueFunctionBinder
{
    [Header("CG&Puzzle")]
    [SerializeField] private CGItem cgPlayer;
    [SerializeField] private HomePuzzleController homePuzzle;

    [Header("Sprites")]
    [SerializeField] private Sprite windowDust;
    [SerializeField] private Sprite windowClean;
    [SerializeField] private Sprite windowPuzzleClear;
    [SerializeField] private Sprite underTheBed;
    [SerializeField] private Sprite underTheBedAlt;
    [SerializeField] private Sprite monsterUnderBed;

    [Header("Puzzles")]
    [SerializeField] private GameObject windowPuzzle;
    [SerializeField] private GameObject diaryPuzzle;
    [SerializeField] private TextAsset magnifierDialogue;

    [Header("Audios")]
    [SerializeField] private AudioClip specialTime;

    [Header("Items")]
    public Item papertowel;
    public Item notes;
    public Item window;
    public Item magnifier;
    public Item under_the_bed;
    public Item sock;
    public Item shovel;

    private const string PAPERTOWEL = "PaperTowel";
    private const string WINDOW_DUST = "window_dust";
    private const string WINDOW_CLEAN = "window_clean";
    private const string WINDOW_CLEAR = "window_clear";
    private const string MAGNIFIER = "Magnifier";
    private const string NORMALBED = "normal_bed";
    private const string UNDERTHEBED = "Under_the_bed";
    private const string SOCK = "sock";
    private const string SHOVEL = "shovel";
    private const string USE_TAG = "can_use";

    private List<IItemUseRule> rules = new List<IItemUseRule>();
    private AudioSource _aud;

    private void OnEnable()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnItemTagChanged += HandleItemTagChanged;
            DialogueManager.Instance.OnVariableChanged += HandleVariableChanged;
            DialogueManager.Instance.OnCutsceneTriggered += HandleCutscene;
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
            DialogueManager.Instance.OnCutsceneTriggered -= HandleCutscene;
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
        under_the_bed.item_Sprite = underTheBed;
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
            case (WINDOW_CLEAR):
                window.item_Sprite = windowPuzzleClear;
                cgPlayer.CGDisplay(window);
                break;
            case (MAGNIFIER):
                window.item_Sprite = windowPuzzleClear;
                InventoryManager.Instance.QueueItem(magnifier);
                break;
            case (NORMALBED):
                under_the_bed.item_Sprite = underTheBed;
                cgPlayer.CGDisplay(under_the_bed);
                break;
            case (UNDERTHEBED):
                _aud.Pause();
                _aud.clip = specialTime;
                under_the_bed.item_Sprite = underTheBedAlt;
                cgPlayer.CGDisplay(under_the_bed);
                _aud.Play();
                break;
            case (SOCK):
                cgPlayer.DisplayItemInfo(sock);
                InventoryManager.Instance.QueueItem(sock);
                break;
            case (SHOVEL):
                Debug.Log("Adding shovel");
                cgPlayer.DisplayItemInfo(shovel);
                InventoryManager.Instance.QueueItem(shovel);
                break;
            case ("clearItemOnly"):
                cgPlayer.ClearItemOnly();
                break;
            case ("clearDisplay"):
                cgPlayer.ClearDisplay();
                break;
        }
    }

    private void HandleVariableChanged(string name, Ink.Runtime.Object value)
    {
        bool boolValue = (bool)value;

        switch (name)
        {
            case "read_notes":

                if (boolValue)
                    cgPlayer.InspectItem(notes);
                else
                    cgPlayer.ClearInspect();
                break;


            case USE_TAG:

                Debug.Log($"can use = {boolValue}");
                break;

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
        homePuzzle.Open();
    }

    private void HandlePuzzleCompleted()
    {
        window.item_Sprite = windowPuzzleClear;
        cgPlayer.CGDisplay(window);

        DialogueManager.Instance.NewStory(magnifierDialogue);

        DialogueManager.Instance.OnDialogueStatusChanged += HandlePuzzleDialogueFinished;
    }

    private void HandlePuzzleDialogueFinished(bool isPlaying)
    {
        if (isPlaying)
            return;

        DialogueManager.Instance.OnDialogueStatusChanged -= HandlePuzzleDialogueFinished;

        windowPuzzle.SetActive(false);
        homePuzzle.Close();
    }


    private void HandleCutscene(string cutsceneID)
    {
        switch (cutsceneID)
        {
            case "show_monster":
                StartCoroutine(ShowMonsterCutscene());
                break;
        }
    }

    private IEnumerator ShowMonsterCutscene()
    {
        DialogueManager.Instance.PauseDialogue();

        GameManager.instance.PushState(GameStateType.Cutscene);

        yield return new WaitForSecondsRealtime(2f);

        under_the_bed.item_Sprite = monsterUnderBed;
        cgPlayer.CGDisplay(under_the_bed);

        yield return new WaitForSecondsRealtime(3f);

        GameManager.instance.PopState(GameStateType.Cutscene);

        DialogueManager.Instance.ResumeDialogue();
    }

    public void ReadDiary()
    {
        diaryPuzzle.SetActive(true);
        homePuzzle.Open();
    }

    public void WriteDiary()
    {
        BookContents diary = diaryPuzzle.GetComponent<BookContents>();
        diary.WriteNewDiary();
        Debug.Log("you wrote some diary");
    }

    public void BindFunctions(Story story)
    {
        story.BindExternalFunction(
            "CanUseItem",
            () => CanUseItem()
        );

        story.BindExternalFunction(
            "UseItem",
            () =>
            {
                TryUseItem();

                InventoryManager.Instance.UseItem(
                    InventoryManager.Instance.GetCurrentItem()
                );
            });

        story.BindExternalFunction(
            "ReadDiary",
            () => ReadDiary()
        );

        story.BindExternalFunction(
            "WriteDiary",
            () => WriteDiary());
    }
}
