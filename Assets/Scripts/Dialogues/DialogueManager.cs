using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    //public instance to be retrived from other scripts
    public static DialogueManager Instance { get; private set; }

    public event System.Action<string> OnItemTagChanged;

    public System.Action<bool> OnDialogueStatusChanged;

    public event System.Action<string, Ink.Runtime.Object> OnVariableChanged;
    public event System.Action<string> OnCutsceneTriggered;
    public event System.Action<string> OnPortraitTagChanged;


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
    private AudioSource _audio;
    private DialogueVariables dialogueVariables;
    private string lastItemTag = "";
    private string lastPortraitTag = "";
    private string lastPlayedTag = "";
    private string lastCutSceneTag = "";

    // variable for the load_globals.ink JSON
    [Header("Load Globals JSON")]
    [SerializeField] private TextAsset loadGlobalsJSON;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI textToDisplay;
    [SerializeField] private GameObject indication;
    [SerializeField] private TextMeshProUGUI speakerLabel;

    [Header("Choices UI")]
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject buttonGroup;

    [Header("Sound FX")]
    [SerializeField] private AudioClip endSFX;

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
        dialogueVariables = new DialogueVariables(loadGlobalsJSON);
    }

    private void Start()
    {
        //find animation and audio componenets in the attached object
        _anim = GameObject.Find("DialoguePanel").GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();

        if(_anim == null)
        {
            Debug.Log("Animator component cannot be found in dialogue manager");
            return;
        }
        if (_audio == null)
        {
            Debug.Log("Audio source cannot be found in dialogue manager");
            return;
        }

        //initialize the dialogue panel
        dialoguePanel.SetActive(false);
        textToDisplay.text = string.Empty;
        indication.SetActive(false);

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

        // contiue story upon user input if there's no choices in current line
        if (_inkstory.currentChoices.Count == 0 &&
            !animPlaying &&
           (InputManager.Instance.IsSubmitPressed()))
        {
            ContinueStory();
        }

        // continue to next line directly if there's no content in current line
        if (string.IsNullOrEmpty(_inkstory.currentText))
        {
            ContinueStory();
        }

        // set the indication active only when the story can be continue
        if (!_inkstory.canContinue)
        {
            indication.SetActive(false);
        }
        else
        {
            indication.SetActive(true);
        }

        // set the speaker label. if the speaker is empty, set the label to
        //empty string
        if (speakerLabel != null && !string.IsNullOrEmpty(GetSpeakerTag()))
        {
            speakerLabel.text = GetSpeakerTag();
        }
        else if (string.IsNullOrEmpty(GetSpeakerTag()))
        {
            speakerLabel.text = "";
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

    public string GetExpressionTag() //get tag of the expression in current line
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
        BindExternalFunctions();
        dialogueVariables.StartListening(_inkstory);

        EnterDialogueMode();
    }

    //enter dialogue
    private void EnterDialogueMode()
    {
        dialoguePanel.SetActive(true);
        textToDisplay.enabled = true;
        isDialoguePlaying = true;
        OnDialogueStatusChanged?.Invoke(true);

        ContinueStory();
        _anim.SetTrigger("dialogueStart");
    }

    //exit dialogue mode
    private IEnumerator ExitDialogueMode()
    {
        _anim.SetTrigger("dialogueEnd");
        animPlaying = true;
        _audio.PlayOneShot(endSFX);

        while (!_anim.GetCurrentAnimatorStateInfo(0).IsName("outroAnim"))
            yield return null;

        animPlaying = false;
        isDialoguePlaying = false;
        OnDialogueStatusChanged?.Invoke(false);

        dialogueVariables.StopListening(_inkstory);
        dialoguePanel.SetActive(false);
        textToDisplay.enabled = false;

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
            textToDisplay.text = _inkstory.Continue();
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

        InputManager.Instance.RegisterSubmitPressed();
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
        string currentPortraitTag = string.Empty;
        string currentCutsceneTag = string.Empty;

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
        //only notify when the tag changed
        if (currentItemTag != lastItemTag)
        {
            lastItemTag = currentItemTag;
            OnItemTagChanged?.Invoke(currentItemTag);
        }

        if (currentPortraitTag != lastPortraitTag)
        {
            lastPortraitTag = currentPortraitTag;
            OnPortraitTagChanged?.Invoke(currentPortraitTag);
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
    public Ink.Runtime.Object GetVariableState(string variableName)
    {
        Ink.Runtime.Object variableValue = null;
        dialogueVariables.variables.TryGetValue(variableName, out variableValue);
        if (variableValue == null)
        {
            Debug.LogWarning("Ink Variable was found to be null: " + variableName);
        }
        return variableValue;
    }

    public void RaiseVariableChaned(string name, Ink.Runtime.Object value)
    {
        OnVariableChanged?.Invoke(name, value);
    }

    public void SetBoolVariable(string variable, bool value)
    {
        if(variable == null || string.IsNullOrEmpty(variable))
        {
            return;
        }

        _inkstory.variablesState[variable] = value;

        Debug.Log("the variable" + variable + "has been set to" + value);
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

    public void ResumeDialogue()
    {
        dialoguePanel.SetActive(true);
        _anim.SetTrigger("dialogueStart");

        cutscenePlaying = false;
    }

    private IEnumerator PauseDialogueCoroutine()
    {
        _anim.SetTrigger("dialogueEnd");
        _audio.PlayOneShot(endSFX);

        while (!_anim.GetCurrentAnimatorStateInfo(0).IsName("outroAnim"))
            yield return null;
        dialoguePanel.SetActive(false);
    }
}
