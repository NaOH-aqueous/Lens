using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    //public instance to be retrived from other scripts
    public static DialogueManager Instance { get; private set; }
    public bool IsReady { get; private set; }
    public bool SaveComplete { get; private set; }

    public event System.Action<string> OnItemTagChanged;

    public System.Action<bool> OnDialogueStatusChanged;

    public event System.Action<string, Ink.Runtime.Object> OnVariableChanged;
    public event System.Action<string> OnCutsceneTriggered;
    public event System.Action<string, string> OnPortraitTagChanged;


    //private ink integrating variables
    Story _inkstory;
    private bool isDialoguePlaying = false; //check if there's any dialogue played

    private bool isChoicesDiaplayed = false;
    private bool animPlaying = false;
    private bool cutscenePlaying;

    //private tag-related variables
    private List<string> tags = new List<string>();
    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";
    private const string AUDIO_TAG = "audio";
    private const string ITEM_TAG = "item";
    private const string CUTSCENE_TAG = "cutscene";

    //private audio and animation variables
    private Animator _anim;
    private DialogueVariables dialogueVariables;
    private string lastItemTag = "";
    private string lastPortraitTag = "";
    private string lastPlayedTag = "";
    private string lastCutSceneTag = "";

    [Header("Dialogue UI")]
     private GameObject dialoguePanel;
     private TextMeshProUGUI textToDisplay;
     private GameObject indication;
     private Image nameLabel;
     private TextMeshProUGUI speakerLabel;
     private GameObject buttonGroup;
     private DialogueUI ui;

    [Header("Choices UI")]
    private GameObject buttonPrefab;

    private void Awake()
    {
        //make it a singleton gameobject
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        EventSystem.current.SetSelectedGameObject(null);
    }

    private void Start()
    {
        // If a DialogueUI already exists in this scene, use it.
        DialogueUI existingUI = FindFirstObjectByType<DialogueUI>();
        if (existingUI != null)
        {
            AssignUI(existingUI);
            return;
        }

        InitializeUI();
    }

    public void InitializeUI()
    {
        IsReady = false;

        // Defensive initialization so Start only needs to run once and
        // AssignUI can reinitialize on scene loads.
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        if (textToDisplay != null)
        {
            textToDisplay.text = string.Empty;
            textToDisplay.enabled = false;
        }

        if (indication != null)
        {
            indication.SetActive(false);
        }

        if (nameLabel != null)
        {
            nameLabel.enabled = false;
        }

        IsReady = true;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // unsubscribe if object is destroyed
        if (InputManager.Instance != null)
        {
            InputManager.Instance.SubmitPerformed -= OnSubmitPerformed;
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        DialogueUI dialogueUI = FindFirstObjectByType<DialogueUI>();

        if (dialogueUI != null)
        {
            AssignUI(dialogueUI);
        }
    }

    private void Update()
    {
        // return right away if dialogue isn't playing
        if (!isDialoguePlaying)
        {
            return;
        }

        if (cutscenePlaying)
        {
            return;
        }

        // continue to next line directly if there's no content in current line
        if (string.IsNullOrEmpty(_inkstory.currentText))
        {
            ContinueStory();
        }

        // set the indication active only when the story can be continue
        if (!_inkstory.canContinue)
        {
            if (indication != null) indication.SetActive(false);
        }
        else
        {
            if (indication != null) indication.SetActive(true);
        }

        // set the speaker label. if the speaker is empty, set the label to
        //empty string
        if (speakerLabel != null && !string.IsNullOrEmpty(GetSpeakerTag()))
        {
            if (nameLabel != null) nameLabel.enabled = true;
            speakerLabel.text = GetSpeakerTag();
        }
        else if (string.IsNullOrEmpty(GetSpeakerTag()))
        {
            if (nameLabel != null) nameLabel.enabled = false;
            if (speakerLabel != null) speakerLabel.text = "";
        }
    }   
    private string[] ParseTags(string tag) //return the parsed tags
    {
        // parse the tag
        string[] splitTag = tag.Split(':');
        if (splitTag.Length != 2)
        {
            Debug.LogError("Tag could not be appropriately parsed: " + tag);
        }
        return splitTag;
    }

    public string GetSpeakerTag() //get tag of the speaker in current line
    {
        foreach (string tag in tags)
        {
            string[] splitTag = ParseTags(tag);
            if (splitTag[0] == SPEAKER_TAG)
            {
                return splitTag[1];
            }
        }
        return "";
    }

    public string GetExpressionTag() //get tag of the speaker in current line
    {
        foreach (string tag in tags)
        {
            string[] splitTag = ParseTags(tag);
            if (splitTag[0] == PORTRAIT_TAG)
            {
                return splitTag[1];
            }
        }
        return "";
    }

    public string GetAudioTag() //get tag of the audio in current line
    {
        foreach (string tag in tags)
        {
            string[] splitTag = ParseTags(tag);
            if (splitTag[0] == AUDIO_TAG)
            {
                return splitTag[1];
            }
        }
        return "";
    }

    public string GetItemTag()
    {
        foreach (string tag in tags)
        {
            string[] splitTag = ParseTags(tag);
            if (splitTag[0] == ITEM_TAG)
            {
                return splitTag[1];
            }
        }
        return "";
    }

    //set the inkasset as current story to the manager
    public void NewStory(TextAsset story)
    {
        _inkstory = new Ink.Runtime.Story(story.text);

        // Load previously saved story state for the current active scene (if any)

        BindExternalFunctions();
        dialogueVariables.StartListening(_inkstory);

        EnterDialogueMode();
    }

    //enter dialogue
    private void EnterDialogueMode()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (textToDisplay != null) textToDisplay.enabled = true;
        isDialoguePlaying = true;

        OnDialogueStatusChanged?.Invoke(true);

        // subscribe to submit event so we stop polling
        if (InputManager.Instance != null)
        {
            InputManager.Instance.SubmitPerformed -= OnSubmitPerformed; // defensive unsubscribe
            InputManager.Instance.SubmitPerformed += OnSubmitPerformed;
        }
        else
        {
            Debug.LogWarning("DialogueManager: InputManager instance missing when entering dialogue mode.");
        }

        ContinueStory();

        if (_anim != null)
            _anim.SetTrigger("dialogueStart");
    }

    //exit dialogue mode
    private IEnumerator ExitDialogueMode()
    {
        Debug.Log($"ExitDialogueMode called. canContinue={_inkstory?.canContinue}");
        // Use a safe helper that ensures the outro state is actually entered (or times out).
        if (_anim != null)
        {
            // trigger the animator and wait for the outro to start/finish with a timeout
            _anim.SetTrigger("dialogueEnd");
            yield return WaitForOutroAnim();
        }

        animPlaying = false;
        isDialoguePlaying = false;
        OnDialogueStatusChanged?.Invoke(false);

        // unsubscribe from submit event
        if (InputManager.Instance != null)
        {
            InputManager.Instance.SubmitPerformed -= OnSubmitPerformed;
        }

        dialogueVariables.StopListening(_inkstory);
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (textToDisplay != null) textToDisplay.enabled = false;

        if (GameManager.instance.CurrentState == GameStateType.Inventory)
        {
            StartCoroutine(InventoryManager.Instance.SelectFirstSlotNextFrame());
        }
    }

    //continue to the next line of the story
    //If there is choices, display them
    //exit the dialogue if there's no more story to continue
    public void ContinueStory()
    {
        if (_inkstory.canContinue)
        {
            if (textToDisplay != null) textToDisplay.text = _inkstory.Continue();
            lastPlayedTag = "";
            GetTags();

            DisplayChoices();
        }
        else
        {
            StartCoroutine(ExitDialogueMode());
        }
    }

    //display the choices and instatiate the buttons according to the num of choices
    private void DisplayChoices()
    {
        if (_inkstory.currentChoices.Count > 0)
        {
            isChoicesDiaplayed = true;

            for (int i = 0; i < _inkstory.currentChoices.Count; ++i)
            {
                //display all current choices
                Ink.Runtime.Choice choice = _inkstory.currentChoices[i];
                //Debug.Log("Choice " + (i + 1) + ". " + choice.text);

                //buttons are initiated according to the num of choices available
                GameObject newButton = Instantiate(buttonPrefab, buttonGroup.transform, false);
                newButton.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = choice.text;

                int currentButton = i;

                //add listener to the current button
                newButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    //Debug.Log("Clicked: " + currentButton);

                    //make the choice according to the index of button

                    StartCoroutine(MakeChoices(currentButton));
                });

                //make the first button default
                if (i == 0)
                {
                    StartCoroutine(SelectFirstButton(newButton));
                }
            }
        }
    }

    //select the first choice button by default
    private IEnumerator SelectFirstButton(GameObject firstButton)
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(firstButton);
    }

    //select the choice from the list and to continue to corresponding dialogues
    private IEnumerator MakeChoices(int currentIndex)
    {   
        yield return new WaitForSecondsRealtime(0.1f);
        _inkstory.ChooseChoiceIndex(currentIndex);
        GameObject[] choicesButtons = GameObject.FindGameObjectsWithTag("ChoiceButton");
        foreach (GameObject choice in choicesButtons)
        {
            Destroy(choice);
        }

        isChoicesDiaplayed = false;

        if (_inkstory.canContinue)
        {
            ContinueStory();
        }

    }

    private void GetTags() //get tags in current line
    {
        tags = _inkstory.currentTags;

        string currentItemTag = string.Empty;
        string currentSpeakerTag = string.Empty;
        string currentCutsceneTag = string.Empty;
        string currentPortraitTag = string.Empty;

        foreach (string tag in tags)
        {
            string[] splitTag = ParseTags(tag);
            switch (splitTag[0])
            {
                case ITEM_TAG:
                    currentItemTag = splitTag[1];
                    break;
                case SPEAKER_TAG:
                    currentSpeakerTag = splitTag[1];
                    break;
                case PORTRAIT_TAG:
                    currentPortraitTag = splitTag[1];
                    break;
                case CUTSCENE_TAG:
                    currentCutsceneTag = splitTag[1];
                    break;
                case AUDIO_TAG:
                    AudioManager.Instance.Play(splitTag[1]);
                    break;
            }
        }

        OnPortraitTagChanged?.Invoke(currentSpeakerTag, currentPortraitTag);

        //only notify when the tag changed
        if (currentItemTag != lastItemTag)
        {
            lastItemTag = currentItemTag;
            OnItemTagChanged?.Invoke(currentItemTag);
        }

        if(currentCutsceneTag != lastCutSceneTag)
        {
            lastCutSceneTag = currentCutsceneTag;
            OnCutsceneTriggered?.Invoke(currentCutsceneTag);
        }

    }

    public bool CheckDialoguePlaying() //Check if current dialogue is playing
    {
        return isDialoguePlaying;
    }

    public bool CheckChoicesDisplay() //Check if any choices are displayed
    {
        return isChoicesDiaplayed;
    }

    //get current variable as well as value
    public bool GetBoolVariable(string variableName)
    {
        if (dialogueVariables.variables.TryGetValue(variableName, out Ink.Runtime.Object variableValue))
        {
            if (variableValue is Ink.Runtime.BoolValue boolValue)
            {
                return boolValue.value;
            }
        }

        Debug.LogWarning("Ink Bool Variable was not found: " + variableName);
        return false;
    }

    public void RaiseVariableChaned(string name, Ink.Runtime.Object value)
    {
        OnVariableChanged?.Invoke(name, value);
    }

    public void SetBoolVariable(string variable, bool value)
    {
        var inkValue = new Ink.Runtime.BoolValue(value);

        dialogueVariables.variables[variable] = inkValue;

        if (_inkstory != null)
        {
            _inkstory.variablesState.SetGlobal(variable, inkValue);
        }

        RaiseVariableChaned(variable,inkValue);
    }

    public void BindExternalFunctions()
    {
        IDialogueFunctionBinder[] binders =
            FindObjectsByType<MonoBehaviour>(
                FindObjectsSortMode.None)
            .OfType<IDialogueFunctionBinder>()
            .ToArray();

        foreach (var binder in binders)
        {
            binder.BindFunctions(_inkstory);
        }
    }

    public void PauseDialogue()
    {
        cutscenePlaying = true;

        StartCoroutine(PauseDialogueCoroutine());
    }

    public void PauseWithoutAnim()
    {
        InputRouter.Instance.PushLayer(InputLayer.Cutscene);
        cutscenePlaying = true;
    }

    public void ResumeWithoutAnim(bool autoPlay)
    {
        InputRouter.Instance.PopLayer(InputLayer.Cutscene);
        cutscenePlaying = false;

        if (autoPlay)
        {
            ContinueStory();
        }
    }

    public void ResumeDialogue()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (_anim != null)
            _anim.SetTrigger("dialogueStart");

        cutscenePlaying = false;
        InputRouter.Instance.PopLayer(InputLayer.Cutscene);
    }

    private IEnumerator PauseDialogueCoroutine()
    {
        InputRouter.Instance.PushLayer(InputLayer.Cutscene);
        if (_anim != null)
        {
            _anim.SetTrigger("dialogueEnd");
            yield return WaitForOutroAnim();
        }

        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    private void OnSubmitPerformed()
    {
        if (!isDialoguePlaying)
            return;

        if (cutscenePlaying)
            return;

        if (_inkstory == null)
            return;

        // Only continue when there are no choices and no animation playing
        if (_inkstory.currentChoices.Count == 0 && !animPlaying)
        {
            ContinueStory();
        }
    }

    public void AssignUI(DialogueUI newUI)
    {
        ui = newUI;

        dialoguePanel = ui.dialoguePanel;
        textToDisplay = ui.textToDisplay;
        indication = ui.indication;
        nameLabel = ui.nameLabel;
        speakerLabel = ui.speakerLabel;
        buttonGroup = ui.buttonGroup;
        buttonPrefab = ui.buttonPrefab;

        if (dialoguePanel != null)
        {
            _anim = dialoguePanel.GetComponent<Animator>();
        }

        InitializeUI();
    }

    private IEnumerator WaitForOutroAnim()
    {
        const int layer = 0;

        animPlaying = true;

        // Timeout to avoid hangs (seconds, unscaled so it's frame-rate independent).
        float timeout = 2.0f;
        float timer = 0f;

        // Wait for the animator to actually enter the state (or timeout).
        while (!_anim.GetCurrentAnimatorStateInfo(layer).IsName("outroAnim") && timer < timeout)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        if (_anim.GetCurrentAnimatorStateInfo(layer).IsName("outroAnim"))
        {
            // Wait until the state completes or until timeout is reached.
            while (_anim.GetCurrentAnimatorStateInfo(layer).IsName("outroAnim") &&
                   _anim.GetCurrentAnimatorStateInfo(layer).normalizedTime < 1f &&
                   timer < timeout)
            {
                timer += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        animPlaying = false;
    }

    private void OnApplicationQuit()
    {
        dialogueVariables.SaveVariables();
    }

    public void SwitchGlobals(TextAsset newGlobalsJSON)
    {
        SaveComplete = false;
        dialogueVariables?.SaveVariables();
        dialogueVariables = new DialogueVariables(newGlobalsJSON);
        SaveComplete = true;
    }

    public void CancelDialogueImmediately()
    {
        StopAllCoroutines();

        isDialoguePlaying = false;
        cutscenePlaying = false;
        animPlaying = false;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (InputManager.Instance != null)
            InputManager.Instance.SubmitPerformed -= OnSubmitPerformed;
    }
}
