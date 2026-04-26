using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
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

    // UI references
    public GameObject mainMenuUI;
    public GameObject pauseMenuUI;
    public GameObject inventoryUI;
    public GameObject dialogueUI;

    public GameStateType currentState { get; private set; }
    public BlurEffect blurVFX;

    private DialogueManager m_dialogueManager;

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
        m_dialogueManager = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();
        ChangeState(GameStateType.Playing);
        blurVFX.enabled = false;
    }

    public void ChangeState(GameStateType newState)
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
    public void TransitionToState(GameStateType newState)
    {
        if (newState == GameStateType.Playing)
        {
            StartCoroutine(blurVFX.IntroTransition());
        }
        else if(newState == GameStateType.Inventory ||
            newState == GameStateType.ItemDisplay)
        {
            blurVFX.OutroTransition();
        }

        currentState = newState;
        HandleStateChange();
    }

    private void HandleStateChange()
    {

        HideAllMenu();

        switch (currentState)
        {
            case GameStateType.Playing:
                AudioListener.pause = false;
                Time.timeScale = 1f; // Resume the game
                inventoryUI.GetComponent<CanvasGroup>().interactable = false;
                dialogueUI.GetComponent<CanvasGroup>().interactable = true;
                break;
            case GameStateType.MainMenu:
                Time.timeScale = 0f; 
                mainMenuUI.SetActive(true);
                break;
            case GameStateType.Paused:
                Time.timeScale = 0f; // Pause the game
                pauseMenuUI.SetActive(true);
                AudioListener.pause = true;
                Button pauseButton = pauseMenuUI.GetComponentInChildren<Button>();
                EventSystem.current.SetSelectedGameObject(pauseButton.gameObject);
                // Test
                break;
            case GameStateType.Inventory:
                AudioListener.pause = false;
                Time.timeScale = 0f;
                inventoryUI.GetComponent<CanvasGroup>().interactable = true;
                if (!m_dialogueManager.CheckDialoguePlaying())
                {
                    dialogueUI.GetComponent<CanvasGroup>().interactable = false;
                }
                else
                {
                    dialogueUI.GetComponent<CanvasGroup>().interactable = true;
                }
                    break;
            case GameStateType.ItemDisplay:
                AudioListener.pause = false;
                Time.timeScale = 0f;
                break;
        }
    }

    private void HideAllMenu()
    {
        mainMenuUI.SetActive(false);
        pauseMenuUI.SetActive(false);
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