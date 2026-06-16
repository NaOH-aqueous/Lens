using Ink.Runtime;
using JetBrains.Annotations;
using MaskTransitions;
using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HomeItemController : MonoBehaviour, IDialogueFunctionBinder
{
    [Header("CG&Puzzle")]
    [SerializeField] private CGItem cgPlayer;
    [SerializeField] private HomePuzzleController homePuzzle;
    [SerializeField] private HomeLensController lens;
    [SerializeField] private Transform focusTargetAlarm;

    [Header("Sprites")]
    [SerializeField] private Sprite windowDust;
    [SerializeField] private Sprite windowClean;
    [SerializeField] private Sprite windowPuzzleClear;
    [SerializeField] private Sprite underTheBed;
    [SerializeField] private Sprite underTheBedAlt;
    [SerializeField] private Sprite monsterUnderBed;
    [SerializeField] private Sprite planterOnly;
    [SerializeField] private Sprite planterWithFlower;
    [SerializeField] private Sprite desk_normal;
    [SerializeField] private Sprite desk_withoutPlanter;
    [SerializeField] private Sprite door_locked;
    [SerializeField] private Sprite door_opened;
    [SerializeField] private Material mat_default;
    [SerializeField] private Material door_emission;
    [SerializeField] private SpriteRenderer deskRenderer;
    [SerializeField] private SpriteRenderer doorRenderer;


    [Header("Puzzles")]
    [SerializeField] private GameObject windowPuzzle;
    [SerializeField] private GameObject diaryPuzzle;
    [SerializeField] private GameObject plantPuzzle;
    [SerializeField] private GameObject letter;
    [SerializeField] private GameObject bg;

    [Header("Dialogues")]
    [SerializeField] private TextAsset introDialogue;
    [SerializeField] private TextAsset magnifierDialogue;
    [SerializeField] private TextAsset origamiPlantingDialogue;
    [SerializeField] private TextAsset plantGrowingDialogue;
    [SerializeField] private TextAsset pixieDialogue;
    [SerializeField] private TextAsset pixieFeedDialogue;
    [SerializeField] private TextAsset keyGetDialogue;
    [SerializeField] private TextAsset endSceneDialogue;
    [SerializeField] private TextAsset endDemoDialogue;

    [Header("Audios")]
    [SerializeField] private AudioClip specialTime;
    [SerializeField] private AudioClip home;

    [Header("Items")]
    public Item papertowel;
    public Item notes;
    public Item window;
    public Item magnifier;
    public Item under_the_bed;
    public Item sock;
    public Item shovel;
    public Item stackPaper;
    public Item origamiFlower;
    public Item planter;
    public Item strangeFruit;
    public Item corner;
    public Item key;
    public Item time;

    private const string PAPERTOWEL = "paper towel";
    private const string WINDOW_DUST = "window_dust";
    private const string WINDOW_CLEAN = "window_clean";
    private const string WINDOW_CLEAR = "window_clear";
    private const string MAGNIFIER = "Magnifier";
    private const string NORMALBED = "normal_bed";
    private const string UNDERTHEBED = "Under_the_bed";
    private const string SOCK = "sock";
    private const string SHOVEL = "shovel";
    private const string USE_TAG = "can_use";
    private const string STACKPAPER = "stack of paper";
    private const string PLANTER = "planter";
    private const string PLANTER_WITH_FLOWER = "planter_with_flower";
    private const string ORIGAMI = "origami flower";
    private const string FRUIT = "strange fruit";
    private const string CORNER = "corner of the room";
    private const string KEY = "key";
    private const string TIME = "time";

    private List<IItemUseRule> rules = new List<IItemUseRule>();
    private AudioSource _aud;
    private bool isIntroPlayed = false;
    private bool isFruitFed = false;

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
        if (lens != null)
        {
            lens.OnLensTriggered += HandleLensTriggered;
            lens.OnItemTriggered += HandleItemTriggered;
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
        if (lens != null)
        {
            lens.OnLensTriggered -= HandleLensTriggered;
            lens.OnItemTriggered -= HandleItemTriggered;
        }
    }

    private void Awake()
    {
        rules.Add(new PaperTowelOnWindowRule());
        rules.Add(new FoldPaperRule());
        rules.Add(new PlantFlowerRule());
        rules.Add(new ShovelOnPlanterRule());
        rules.Add(new MagnifierOnWall());
        rules.Add(new FruitOnWall());

        window.item_Sprite = windowDust;
        under_the_bed.item_Sprite = underTheBed;
        planter.item_Sprite = planterOnly;
        deskRenderer.sprite = desk_normal;
        doorRenderer.sprite = door_locked;
        doorRenderer.material = mat_default;
    }

    private void Start()
    {
        _aud = GetComponent<AudioSource>();

        if (!isIntroPlayed)
        {
            StartCoroutine(IntroSceneCoroutine());
        }
;    }

    public bool CanUseItem()
    {
        Item inventoryItem =
            InventoryManager.Instance.GetCurrentItem();

        Item worldItem =
            cgPlayer.GetCurrentDisplay();

        Debug.Log(
            $"[CanUseItem] inventory={inventoryItem?.item_Name}, " +
            $"world={worldItem?.item_Name}"
        );

        //return if no selected item
        if (inventoryItem == null)
            return false;

        foreach (var rule in rules)
        {
            bool result =
                rule.CanUse(
                    inventoryItem,
                    worldItem
                );

            Debug.Log(
                $"Rule {rule.GetType().Name} = {result}"
            );

            if (result)
                return true;
        }

        return false;
    }

    public void TryUseItem()
    {
        Item inventoryItem =
            InventoryManager.Instance.GetCurrentItem();

        Item worldItem =
            cgPlayer.GetCurrentDisplay();

        if (inventoryItem == null)
            return;

        foreach (var rule in rules)
        {
            if (rule.CanUse(inventoryItem, worldItem))
            {
                rule.Apply(
                    inventoryItem,
                    worldItem
                );

                if (rule.ConsumeItem)
                {
                    InventoryManager.Instance.UseItem(inventoryItem);
                }

                return;
            }
        }

        Debug.Log(
            $"No rule matched for {inventoryItem.item_Name}"
        );
    }

    private IEnumerator IntroSceneCoroutine()
    {
        bg.SetActive(true);
        yield return null;
        GameManager.instance.PushState(GameStateType.ItemDisplay);
        _aud.Stop();
        yield return null;
        AudioManager.Instance.Play("alarm");
        yield return new WaitForSecondsRealtime(2f);
        DialogueManager.Instance.NewStory(introDialogue);
        DialogueManager.Instance.OnDialogueStatusChanged += HandleIntroDialogueFinished;
    }

    private void HandleIntroDialogueFinished(bool isPlaying)
    {
        TransitionManager.Instance.PlayEndHalfTransition(0.5f);
        bg.SetActive(false);
        _aud.Play();
        GameManager.instance.PopState(GameStateType.ItemDisplay);
        DialogueManager.Instance.OnDialogueStatusChanged -= HandleIntroDialogueFinished;
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
                cgPlayer.PlayTooltip();
                break;
            case (WINDOW_CLEAN):
                window.item_Sprite = windowClean;
                cgPlayer.CGDisplay(window);
                break;
            case (WINDOW_CLEAR):
                window.item_Sprite = windowPuzzleClear;
                cgPlayer.CGDisplay(window);
                cgPlayer.PlayTooltip();
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
                cgPlayer.DisplayItemInfo(shovel);
                InventoryManager.Instance.QueueItem(shovel);
                break;
            case (STACKPAPER):
                InventoryManager.Instance.QueueItem(stackPaper);
                break;
            case (PLANTER_WITH_FLOWER):
                InventoryManager.Instance.UseItem(origamiFlower);
                InventoryManager.Instance.CloseInventory();
                planter.item_Sprite = planterWithFlower;
                cgPlayer.DisplayItemInfo(planter);
                break;
            case (FRUIT):
                GameManager.instance.PopState(GameStateType.Cutscene);
                InventoryManager.Instance.QueueItem(strangeFruit);
                plantPuzzle.SetActive(false);
                _aud.Play();
                break;
            case (KEY):
                InventoryManager.Instance.QueueItem(key);
                break;
            case (TIME):
                InventoryManager.Instance.QueueItem(time);
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

        if(value is Ink.Runtime.BoolValue boolVal)
        {
            switch (name)
            {
                case "read_notes":
                    if (boolVal)
                        cgPlayer.InspectItem(notes);
                    else
                        cgPlayer.ClearInspect();
                    break;
                case "door_unlocked":
                    if (boolVal)
                    {
                        doorRenderer.sprite = door_opened;
                        doorRenderer.material = door_emission;
                    }
                    break;
                case USE_TAG:
                    Debug.Log($"can use = {boolVal}");
                    break;

            }
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
            case "diaryPageRemove":
                StartCoroutine(TakeDiaryPageCutscene());
                break;
            case "plantGrow":
                StartCoroutine(PlantGrowCoroutine());
                break;
            case "zoom_to_alarm":
                StartCoroutine(EndSceneCutscene());
                break;
        }
    }

    private IEnumerator ShowMonsterCutscene()
    {
        DialogueManager.Instance.PauseDialogue();

        GameManager.instance.PushState(GameStateType.Cutscene);
        PortraitManager.Instance.SetCutscenePortraitLock(true);

        yield return new WaitForSecondsRealtime(2f);

        under_the_bed.item_Sprite = monsterUnderBed;
        cgPlayer.CGDisplay(under_the_bed);

        yield return new WaitForSecondsRealtime(3f);

        GameManager.instance.PopState(GameStateType.Cutscene);

        DialogueManager.Instance.ResumeDialogue();
        PortraitManager.Instance.SetCutscenePortraitLock(false);
        DialogueManager.Instance.OnDialogueStatusChanged += HandleMonsterDialogueFinished;
    }

    private void HandleMonsterDialogueFinished(bool isPlaying)
    {
        if (isPlaying)
        {
            return;
        }

        _aud.Pause();
        _aud.clip = home;
        _aud.Play();

        DialogueManager.Instance.OnDialogueStatusChanged -= HandleMonsterDialogueFinished;
    }

    private IEnumerator TakeDiaryPageCutscene()
    {
        homePuzzle.ExitAllPuzzle();
        DialogueManager.Instance.PauseWithoutAnim();

        yield return new WaitForSecondsRealtime(1.5f);
        DialogueManager.Instance.ResumeWithoutAnim(false);
    }

    private IEnumerator EndSceneCutscene()
    {
        PortraitManager.Instance.SetCutscenePortraitLock(true);
        yield return null;
        DialogueManager.Instance.PauseDialogue();
        CameraController.Instance.FocusOn(focusTargetAlarm);

        yield return new WaitForSecondsRealtime(3f);

        PortraitManager.Instance.SetCutscenePortraitLock(false);
        DialogueManager.Instance.ResumeDialogue();

        DialogueManager.Instance.OnDialogueStatusChanged += HandleEndsceneDialogue;
    }

    private void HandleEndsceneDialogue(bool isplaying)
    {
        CameraController.Instance.ReturnToPlayer();

        DialogueManager.Instance.OnDialogueStatusChanged -= HandleEndsceneDialogue;
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
    }

    public void InspectItem()
    {
        StartCoroutine(InspectRoutine());
    }

    private IEnumerator InspectRoutine()
    {
        InventoryManager.Instance.CloseInventory();

        yield return null;

        Item currentItem = InventoryManager.Instance.GetCurrentItem();
        cgPlayer.TransitionToInpsectMode(currentItem);
    }

    public void PlantFlower()
    {
        StartCoroutine(PlantFlowerCoroutine());
    }

    private IEnumerator PlantFlowerCoroutine()
    {
        yield return null;
        InventoryManager.Instance.CloseInventory();
        yield return new WaitForSecondsRealtime(0.5f);
        DialogueManager.Instance.NewStory(origamiPlantingDialogue);
    }

    public void FoldOrigamiFlower()
    { 
        InventoryManager.Instance.QueueItem(origamiFlower);
        DialogueManager.Instance.SetBoolVariable("origami_get", true);
    }

    private void InspectPlanter()
    {
        cgPlayer.DisplayItemInfo(planter);
        cgPlayer.PlayTooltip();
    }

    private void TakeFlower()
    {
        cgPlayer.ClearDisplay();
        deskRenderer.sprite = desk_withoutPlanter;
        InventoryManager.Instance.QueueItem(planter);
    }

    private IEnumerator PlantGrowCoroutine()
    {
        InventoryManager.Instance.UseItem(planter);
        Animator plantAnim = plantPuzzle.GetComponent<Animator>();
        plantAnim.enabled = false;

        GameManager.instance.PushState(GameStateType.Cutscene);
        _aud.Pause();
        plantPuzzle.SetActive(true);

        yield return new WaitForSecondsRealtime(1f);
        plantAnim.enabled = true;
    }

    public void HandlePlantAnimComplete()
    {
        StartCoroutine(HandlePlantAnimCompleteCoroutine());
    }

    private IEnumerator HandlePlantAnimCompleteCoroutine()
    {
        AudioManager.Instance.Play("fairyTone");
        yield return new WaitForSecondsRealtime(1.5f);
        DialogueManager.Instance.NewStory(plantGrowingDialogue);
    }

    private void InspectCorner()
    {
        cgPlayer.CGDisplay(corner);
        cgPlayer.PlayItemDialogue = true;
        cgPlayer.PlayTooltip();
    }

    public void MagnifyCorner()
    {
        StartCoroutine(MagnifierUICoroutine());
        if (!isFruitFed)
        {
            lens.SetPixieAltSprite(true);
            lens.SetPixieLineup(false);
        }
        else
        {
            lens.SetPixieAltSprite(false);
            lens.SetPixieLineup(true);
        }

    }

    private IEnumerator MagnifierUICoroutine()
    {
        yield return null;
        InventoryManager.Instance.CloseInventory();
        yield return null;
        lens.EnableLens();
    }

    private void HandleLensTriggered()
    {
        if(cgPlayer.GetCurrentDisplay() != corner)
        {
            return;
        }

        if (!isFruitFed)
        {
            lens.DisableLens();
            DialogueManager.Instance.NewStory(pixieDialogue);
            DialogueManager.Instance.OnDialogueStatusChanged += HandlePixieDialogueComplete;
        }

    }

    private void HandleItemTriggered(Item item)
    {
        if (cgPlayer.GetCurrentDisplay() != corner)
        {
            return;
        }

        if (item == key)
        {
            lens.DisableLens();
            lens.SetPixieLineup(false);
            lens.SetPixieWithoutKey(true);
            DialogueManager.Instance.NewStory(keyGetDialogue);
            DialogueManager.Instance.OnDialogueStatusChanged += HandlePixieDialogueComplete;
        }
    }

    private void HandlePixieDialogueComplete(bool isplaying)
    {
        lens.EnableLens();
        DialogueManager.Instance.OnDialogueStatusChanged -= HandlePixieDialogueComplete;
    }

    public void HandleFruitFed()
    {
        isFruitFed = true;
        StartCoroutine(StartFruitDialogue());
    }

    private IEnumerator StartFruitDialogue()
    {
        yield return null;
        InventoryManager.Instance.CloseInventory();

        yield return new WaitForSecondsRealtime(0.5f);

        DialogueManager.Instance.NewStory(pixieFeedDialogue);
    }

    private void PlayEndScene()
    {
        StartCoroutine(PlayEndSceneCoroutine());
    }

    private IEnumerator PlayEndSceneCoroutine()
    {
        yield return null;
        _aud.Pause();
        InventoryManager.Instance.UseItem(key);
        yield return new WaitForSecondsRealtime(1f);


        DialogueManager.Instance.NewStory(endSceneDialogue);
    }

    private void EndDemo()
    {
        StartCoroutine(EndDemoCoroutine());
    }

    private IEnumerator EndDemoCoroutine()
    {
        yield return null;
        TransitionManager.Instance.PlayTransition(1.5f);
        yield return new WaitForSecondsRealtime(0.5f);
        bg.SetActive(true);

        yield return new WaitForSecondsRealtime(0.5f);
        DialogueManager.Instance.NewStory(endDemoDialogue);
    }

    private void ReadLetter()
    {
        letter.SetActive(true);
    }

    public void BindFunctions(Story story)
    {
        story.BindExternalFunction(
            "CanUseItem", () => CanUseItem());

        story.BindExternalFunction(
            "UseItem", () => TryUseItem());

        story.BindExternalFunction(
            "ReadDiary", () => ReadDiary());

        story.BindExternalFunction(
            "WriteDiary", () => WriteDiary());

        story.BindExternalFunction(
            "InspectItem", () => InspectItem());

        story.BindExternalFunction(
            "InspectPlanter", () => InspectPlanter());

        story.BindExternalFunction(
            "TakeFlower", () => TakeFlower());

        story.BindExternalFunction(
            "InspectCorner", () => InspectCorner());

        story.BindExternalFunction(
            "EndScene", () => PlayEndScene());

        story.BindExternalFunction(
            "EndDemo", () => EndDemo());

        story.BindExternalFunction(
           "ReadLetter", () => ReadLetter());
    }
}
