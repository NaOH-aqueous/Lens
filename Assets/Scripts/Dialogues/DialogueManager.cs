using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    Story _inkstory;
    private bool isDialoguePlaying = false;
    private bool isChoicesDiaplayed = false;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI textToDisplay;
    [SerializeField] private GameObject indication;

    [Header("Choices UI")]
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject buttonGroup;

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
           ( DialogueInput.Instance.IsSubmitPressed() ||
            DialogueInput.Instance.IsInteractPressed()))
        {
            ContinueStory();
            GetTags();
        }
    }

    public void GetTags()
    {
        List<string> tags = _inkstory.currentTags;
    }

    //set the inkasset as current story to the manager
    public void NewStory(TextAsset story)
    {
        _inkstory = new Story(story.text);
        EnterDialogueMode();
    }

    //enter dialogue
    private void EnterDialogueMode()
    {
        dialoguePanel.SetActive(true);
        textToDisplay.enabled = true;
        isDialoguePlaying = true;
        ContinueStory();

    }

    //exit dialogue after 1 frame
    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForEndOfFrame();

        isDialoguePlaying = false;
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
                Choice choice = _inkstory.currentChoices[i];
                Debug.Log("Choice " + (i + 1) + ". " + choice.text);

                //buttons are initiated according to the num of choices available
                GameObject newButton = Instantiate(buttonPrefab, buttonGroup.transform, false);
                newButton.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = choice.text;

                int currentButton = i;

                //add listener to the current button
                newButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    Debug.Log("Clicked: " + currentButton);

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
        foreach(GameObject choice in choicesButtons) { 
            Destroy(choice);
        }

        DialogueInput.Instance.RegisterSubmitPressed();
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
}
