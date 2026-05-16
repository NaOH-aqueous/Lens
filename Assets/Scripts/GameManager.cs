using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public enum GameStateType
{
    MainMenu,
    Playing,
    Paused,
    Inventory,
    ItemDisplay,
    Puzzle,
    Cutscene
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public System.Action<GameStateType> OnGameStateChanged;

    // UI references
    public GameObject mainMenuUI;
    public GameObject pauseMenuUI;
    public GameObject inventoryUI;
    public GameObject dialogueUI;
    public GameObject cgUI;
    public GameObject puzzleUI;


    private Stack<GameStateType> stateStack = new Stack<GameStateType>();
    public GameStateType CurrentState =>
        stateStack.Count > 0 ? stateStack.Peek() : GameStateType.Playing;

    public BlurEffect blurVFX;

    private DialogueManager m_dialogueManager;

    private CanvasGroup inventoryCanvasGroup;
    private CanvasGroup dialogueCanvasGroup;
    private CanvasGroup cgCanvasGroup;
    private CanvasGroup puzzleCanvasGroup;
    private Button pauseButton;

    private bool isTransitioning;
    private PlayerController m_playerController;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        m_dialogueManager = DialogueManager.Instance;

        inventoryCanvasGroup = inventoryUI.GetComponent<CanvasGroup>();
        dialogueCanvasGroup = dialogueUI.GetComponent<CanvasGroup>();
        cgCanvasGroup = cgUI.GetComponent<CanvasGroup>();
        puzzleCanvasGroup = puzzleUI.GetComponent<CanvasGroup>();

        pauseButton = pauseMenuUI.GetComponentInChildren<Button>();
        m_playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        PushState(GameStateType.Playing);
        blurVFX.enabled = false;

        if (m_dialogueManager != null)
        {
            m_dialogueManager.OnDialogueStatusChanged += HandleDialogueStateChanged;
        }
    }


    private void OnEnable()
    {
        OnGameStateChanged += ApplyState;

    }

    private void OnDisable()
    {
        OnGameStateChanged -= ApplyState;
        if (m_dialogueManager != null)
        {
            m_dialogueManager.OnDialogueStatusChanged -= HandleDialogueStateChanged;
        }
    }

    public void PushState(GameStateType newState)
    {
        if (stateStack.Count > 0 &&
            stateStack.Peek() == newState)
        {
            Debug.Log($"Skipped duplicate push: {newState}");
            return;
        }

        stateStack.Push(newState);

        Debug.Log($"Pushed: {newState}");

        OnGameStateChanged?.Invoke(newState);
    }

    public void PopState(GameStateType expectedState)
    {
        if (stateStack.Count == 0)
            return;

        if (stateStack.Peek() != expectedState)
        {
            Debug.LogWarning($"Tried to pop {expectedState} but top is {stateStack.Peek()}");
            return;
        }

        stateStack.Pop();
        Debug.Log("Popped -> Now: " + CurrentState);
        OnGameStateChanged?.Invoke(CurrentState);
    }

    private IEnumerator HandleModeTransition(GameStateType state)
    {
        if (isTransitioning)
        {
            yield break;
        }

        if (state == GameStateType.Playing)
        {
            isTransitioning = true;
            yield return StartCoroutine(blurVFX.IntroTransition());
            isTransitioning = false;
        }
        else if (state == GameStateType.Inventory ||
                 state == GameStateType.ItemDisplay)
        {
            isTransitioning = true;
            yield return StartCoroutine(blurVFX.OutroTransition());
            isTransitioning = false;
        }
    }

    private void ApplyState(GameStateType currentState)
    {
        HideAllMenu();
        StartCoroutine(HandleModeTransition(currentState));

        switch (currentState)
        {
            case GameStateType.Playing:
                PlayingMode();
                break;
            case GameStateType.MainMenu:
                Time.timeScale = 0f; 
                mainMenuUI.SetActive(true);
                break;
            case GameStateType.Paused:
                PausedMode();
                break;
            case GameStateType.Inventory:
                AudioListener.pause = false;
                Time.timeScale = 0f;
                break;
            case GameStateType.ItemDisplay:
                AudioListener.pause = false;
                Time.timeScale = 0f;
                break;
            case GameStateType.Puzzle:
                AudioListener.pause = false;
                Time.timeScale = 0f;
                break;
            case GameStateType.Cutscene:
                Time.timeScale = 0f;
                break;
        }

        RefreshUIInteractivity();
    }

    private void HandleDialogueStateChanged(bool isDialoguePlaying)
    {
        RefreshUIInteractivity();
    }

    private void RefreshUIInteractivity()
{
    bool dialoguePlaying = m_dialogueManager != null &&
                           m_dialogueManager.CheckDialoguePlaying();

    inventoryCanvasGroup.interactable =
        CurrentState == GameStateType.Inventory && !dialoguePlaying;

    cgCanvasGroup.interactable =
        CurrentState == GameStateType.ItemDisplay && !dialoguePlaying;

    puzzleCanvasGroup.interactable =
        CurrentState == GameStateType.Puzzle && !dialoguePlaying;

    dialogueCanvasGroup.interactable = dialoguePlaying;
}

    private void HideAllMenu()
    {
        mainMenuUI.SetActive(false);
        pauseMenuUI.SetActive(false);
    }

    private void PlayingMode()
    {
        AudioListener.pause = false;
        Time.timeScale = 1f; // Resume the game
        inventoryCanvasGroup.interactable = false;
        dialogueCanvasGroup.interactable = true;
        cgCanvasGroup.interactable = false;
        puzzleCanvasGroup.interactable = false;
    }

    private void PausedMode()
    {
        Time.timeScale = 0f; // Pause the game
        pauseMenuUI.SetActive(true);
        AudioListener.pause = true;
        EventSystem.current.SetSelectedGameObject(pauseButton.gameObject);
    }

    public void QuitGame()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}