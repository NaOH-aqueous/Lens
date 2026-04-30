using System.Collections;
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
    ItemDisplay
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

    public GameStateType currentState { get; private set; }
    public BlurEffect blurVFX;

    private DialogueManager m_dialogueManager;
    private InventoryManager m_inventoryManager;

    private CanvasGroup inventoryCanvasGroup;
    private CanvasGroup dialogueCanvasGroup;
    private Button pauseButton;

    private bool isTransitioning = false;
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
        m_inventoryManager = InventoryManager.Instance;

        inventoryCanvasGroup = inventoryUI.GetComponent<CanvasGroup>();
        dialogueCanvasGroup = dialogueUI.GetComponent<CanvasGroup>();
        pauseButton = pauseMenuUI.GetComponentInChildren<Button>();
        m_playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        ChangeState(GameStateType.Playing);
        blurVFX.enabled = false;
    }

    private void OnEnable()
    {
        OnGameStateChanged += HandleStateChange;
        OnGameStateChanged += HandleModeTransition;
        if (m_dialogueManager != null)
        {
            m_dialogueManager.OnDialogueStatusChanged += HandleDialogueStateChanged;
        }
    }

    private void OnDisable()
    {
        OnGameStateChanged -= HandleStateChange;
        OnGameStateChanged -= HandleModeTransition;
        if (m_dialogueManager != null)
        {
            m_dialogueManager.OnDialogueStatusChanged -= HandleDialogueStateChanged;
        }
    }

    private void ChangeState(GameStateType newState)
    {
        TransitionToState(newState);
    }

    // Click events for UI buttons
    public void ChangeToMainMenu()
    {
        ChangeState(GameStateType.MainMenu);
        // Load the main menu scene
        //SceneManager.LoadScene("MainMenu");
    }

    public void ChangeToPlaying()
    {
        ChangeState(GameStateType.Playing);
    }

    public void ChangeToPaused()
    {
        ChangeState(GameStateType.Paused);
    }

    public void ChangeToInventory()
    {
        ChangeState(GameStateType.Inventory);
    }

    public void ChangeToItemDisplay()
    {
        ChangeState(GameStateType.ItemDisplay);
    }

    public GameStateType GetGameStatus()
    {
        return currentState;
    }
    private void TransitionToState(GameStateType newState)
    {
        //return directly if the state is the same or currently transitioning
        if (isTransitioning)
        {
            return;
        }

        bool stateChanged = currentState != newState;

        if (stateChanged)
        {
            OnGameStateChanged?.Invoke(newState);
        }
        currentState = newState;
    }

    private void HandleModeTransition(GameStateType currentState)
    {
        if (currentState == GameStateType.Playing)
        {
            isTransitioning = true;
            StartCoroutine(blurVFX.IntroTransition());
            isTransitioning = false;
        }
        else if (currentState == GameStateType.Inventory ||
            currentState == GameStateType.ItemDisplay)
        {
            isTransitioning = true;
            StartCoroutine(blurVFX.OutroTransition());
            isTransitioning = false;
        }
    }

    private void HandleStateChange(GameStateType currentState)
    {

        HideAllMenu();

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
                inventoryCanvasGroup.interactable = true;
                break;
            case GameStateType.ItemDisplay:
                AudioListener.pause = false;
                Time.timeScale = 0f;
                break;
        }
    }

    private void HandleDialogueStateChanged(bool isDialoguePlaying)
    {
        // if the current state is inventory
        // set the interactivity of inventory UI to be the opposite of dialogue status
        if (currentState == GameStateType.Inventory)
        {
            inventoryCanvasGroup.interactable = !isDialoguePlaying;
        }
        else
        {
            // if the inventory mode is on, set interactable
            inventoryCanvasGroup.interactable = false;
        }

        //set the dialogue UI interactivity to be the opposite of dialogue status
        dialogueCanvasGroup.interactable = !isDialoguePlaying;
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