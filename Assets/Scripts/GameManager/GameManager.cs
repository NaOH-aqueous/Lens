using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameStateType
{
    MainMenu,
    Playing,
    Paused,
    Inventory,
    ItemDisplay,
    Puzzle,
    Lens,
    Cutscene
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public System.Action<GameStateType> OnGameStateChanged;

    private GameObject pauseMenuUI;
    private GameObject inventoryUI;
    private GameObject dialogueUI;
    private GameObject cgUI;
    private GameObject puzzleUI;
    private GameObject lensUI;

    [Header("Scene Names")]
    public string mainMenuSceneName = "MainMenu";
    public string gameSceneName = "Home";

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
    private PuzzleController puzzleController;
    private LensController lensController;

    private bool isTransitioning;

    // subscription guard for cancel event
    private bool cancelSubscribed = false;

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


        PushState(GameStateType.Playing);
        blurVFX.enabled = false;

        if (m_dialogueManager != null)
        {
            m_dialogueManager.OnDialogueStatusChanged += HandleDialogueStateChanged;
        }

        SubscribeCancel();
    }

    private void OnEnable()
    {
        OnGameStateChanged += ApplyState;
        SubscribeCancel();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        OnGameStateChanged -= ApplyState;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (m_dialogueManager != null)
        {
            m_dialogueManager.OnDialogueStatusChanged -= HandleDialogueStateChanged;
        }

        UnsubscribeCancel();
    }

    private void OnDestroy()
    {
        UnsubscribeCancel();
    }

    private void SubscribeCancel()
    {
        if (cancelSubscribed)
            return;

        if (InputManager.Instance == null)
            return;

        InputManager.Instance.CancelPerformed -= OnCancelPerformed;
        InputManager.Instance.CancelPerformed += OnCancelPerformed;
        cancelSubscribed = true;
    }

    private void UnsubscribeCancel()
    {
        if (!cancelSubscribed)
            return;

        if (InputManager.Instance != null)
            InputManager.Instance.CancelPerformed -= OnCancelPerformed;

        cancelSubscribed = false;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        SceneUIReferences refs = FindFirstObjectByType<SceneUIReferences>();

        if (refs != null)
        {
            pauseMenuUI = refs.pauseMenu;
            inventoryUI = refs.inventoryUI;
            dialogueUI = refs.dialogueUI;
            cgUI = refs.cgUI;
            puzzleUI = refs.puzzleUI;
            lensUI = refs.lensUI;
        }
        else
        {
            Debug.Log("No UI refs in this scene!");
        }

        InitializeSceneReferences();
    }

    private void InitializeSceneReferences()
    {
        inventoryCanvasGroup =
            inventoryUI != null ? inventoryUI.GetComponent<CanvasGroup>() : null;

        dialogueCanvasGroup =
            dialogueUI != null ? dialogueUI.GetComponent<CanvasGroup>() : null;

        cgCanvasGroup =
            cgUI != null ? cgUI.GetComponent<CanvasGroup>() : null;

        puzzleCanvasGroup =
            puzzleUI != null ? puzzleUI.GetComponent<CanvasGroup>() : null;

        puzzleController =
            puzzleUI != null ? puzzleUI.GetComponent<PuzzleController>() : null;

        lensController =
            lensUI != null ? lensUI.GetComponent<LensController>() : null;

        pauseButton =
            pauseMenuUI != null
            ? pauseMenuUI.GetComponentInChildren<Button>()
            : null;

        m_dialogueManager = DialogueManager.Instance;
    }

    private void OnCancelPerformed()
    {
        // delegate to the existing ExitCurrentMode logic
        ExitCurrentMode();
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
                 state == GameStateType.ItemDisplay ||
                 state == GameStateType.Puzzle||
                 state == GameStateType.Cutscene)
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
            case GameStateType.Lens:
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

    public void ExitCurrentMode()
    {
        if (DialogueManager.Instance.CheckDialoguePlaying())
            return;
        switch (CurrentState)
        {
            case GameStateType.Playing:
                StartCoroutine(PauseGame());
                break;
            case GameStateType.Inventory:
                StartCoroutine(InventoryManager.Instance.CloseInventory());
                break;

            case GameStateType.ItemDisplay:
                FindFirstObjectByType<CGItem>().ExitCGMode();
                break;

            case GameStateType.Puzzle:
                puzzleController.ExitAllPuzzle();
                break;

            case GameStateType.Lens:
                lensController.DisableLens();
                break;
        }
    }

    private IEnumerator PauseGame()  //pause the game according to user inputs
    {
        yield return null;
        PushState(GameStateType.Paused);
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        PopState(GameStateType.Paused);
    }

    private void PausedMode()
    {
        Time.timeScale = 0f; // Pause the game
        pauseMenuUI.SetActive(true);
        AudioListener.pause = true;
        EventSystem.current.SetSelectedGameObject(pauseButton.gameObject);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void ReturnToMain()
    {
        if (InventoryManager.Instance != null)
        {
            StartCoroutine(InventoryManager.Instance.CloseInventory());
        }

        if (blurVFX != null)
        {
            blurVFX.enabled = false;
        }

        stateStack.Clear();
        PushState(GameStateType.MainMenu);

        // Load main menu scene
        SceneManager.LoadScene(mainMenuSceneName);
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