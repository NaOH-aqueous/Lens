using Ink.Parsed;
using Ink.Runtime;
using Ink.UnityIntegration;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    //public instance to be retrived from other scripts
    public static DialogueManager Instance { get; private set; }

    //private ink integrating variables
    Ink.Runtime.Story _inkstory;
    private bool isDialoguePlaying = false; //check if there's any dialogue played
    private bool isChoicesDiaplayed = false;

    //private tag-related variables
    private List<string> tags = new List<string>();
    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";
    private const string AUDIO_TAG = "audio";

    //private audio and animation variables
    private Animator _anim;
    private AudioSource _audio;
    private DialogueVariables dialogueVariables;
    private string lastPlayedTag = "";

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
        dialoguePanel.SetActive(false);
        textToDisplay.text = string.Empty;
        indication.SetActive(false);

    }

    private void Update() //singleton class, only have one in the scene
    {
        // return right away if dialogue isn't playing
        if (!isDialoguePlaying)
        {
            return;
        }

        if (_inkstory.currentChoices.Count == 0 &&
           (InputManager.Instance.IsSubmitPressed() ||
            InputManager.Instance.IsInteractPressed()))
        {
            ContinueStory();
        }

        if (string.IsNullOrEmpty(_inkstory.currentText))
        {
            ContinueStory();
        }

        if (!_inkstory.canContinue)
        {
            indication.SetActive(false);
        }

        if (speakerLabel != null && !string.IsNullOrEmpty(GetSpeakerTag()))
        {
            speakerLabel.text = GetSpeakerTag();
        }
        else if(string.IsNullOrEmpty(GetSpeakerTag()))
        {
            speakerLabel.text = "";
        }
    }

    private string[] ParseTags(string tag)
    {
        // parse the tag
        string[] splitTag = tag.Split(':');
        if (splitTag.Length != 2)
        {
            Debug.LogError("Tag could not be appropriately parsed: " + tag);
        }
        return splitTag;
    }

    public string GetSpeakerTag()
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

    public string GetExpressionTag()
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

    public string GetAudioTag()
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

    public void PlaySound(AudioClip sound, string audioName)
    {
        string currentTag = GetAudioTag();

        if (currentTag == audioName && sound != null && lastPlayedTag != audioName)
        {
            _audio.PlayOneShot(sound);
            lastPlayedTag = audioName;
            Debug.Log("sound is playing");
        }
    }

    //set the inkasset as current story to the manager
    public void NewStory(TextAsset story)
    {
        _inkstory = new Ink.Runtime.Story(story.text);
        dialogueVariables.StartListening(_inkstory);

        EnterDialogueMode();
    }

    //enter dialogue
    private void EnterDialogueMode()
    {
        dialoguePanel.SetActive(true);
        textToDisplay.enabled = true;
        isDialoguePlaying = true;

        ContinueStory();
        _anim.SetTrigger("dialogueStart");
    }

    //exit dialogue after 1 frame
    private IEnumerator ExitDialogueMode()
    {
        _anim.SetTrigger("dialogueEnd");
        _audio.PlayOneShot(endSFX);
        isDialoguePlaying = false;
        yield return new WaitForSeconds(0.6f);

        dialogueVariables.StopListening(_inkstory);
        dialoguePanel.SetActive(false);
        textToDisplay.enabled = false;
    }

    //continue to the next line of the story
    //If there is choices, display them
    //exit the dialogue if there's no more story to continue
    public void ContinueStory()
    {
        if (_inkstory.canContinue)
        {
            indication.SetActive(true);
            textToDisplay.text = _inkstory.Continue();
            lastPlayedTag = "";
            GetTags();

            DisplayChoices();
        }
        else
        {
            indication.SetActive(false);
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

                    MakeChoices(currentButton);
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
    private void MakeChoices(int currentIndex)
    {
        _inkstory.ChooseChoiceIndex(currentIndex);

        GameObject[] choicesButtons = GameObject.FindGameObjectsWithTag("ChoiceButton");
        foreach (GameObject choice in choicesButtons)
        {
            Destroy(choice);
        }

        InputManager.Instance.RegisterSubmitPressed();
        isChoicesDiaplayed = false;
        ContinueStory();
    }

    public bool CheckDialoguePlaying()
    {
        return isDialoguePlaying;
    }

    public bool CheckChoicesDisplay()
    {
        return isChoicesDiaplayed;
    }

    public void GetTags()
    {
        tags = _inkstory.currentTags;
    }

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
}
