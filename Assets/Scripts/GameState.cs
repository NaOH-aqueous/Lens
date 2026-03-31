using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public enum GameStateType
{
    MainMenu,
    Playing,
    Paused
}

public class GameState : MonoBehaviour
{
    public static GameState instance;

    // UI references
    public GameObject mainMenuUI;
    public GameObject playingUI;
    public GameObject pauseMenuUI;

    // Delay before changing states (in seconds)
    public int delay = 1;

    public GameStateType currentState { get; private set; }

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
        //ChangeState(GameStateType.Paused);
    }

    public void ChangeState(GameStateType newState)
    {
        StartCoroutine(TransitionToState(newState));
    }

    // Click events for UI buttons
    public void ChangeToMainMenu()
    {
        ChangeState(GameStateType.MainMenu);
    }

    public void ChangeToPlaying()
    {
        ChangeState(GameStateType.Playing);
    }

    public void ChangeToPaused()
    {
        ChangeState(GameStateType.Paused);
    }
    

    private IEnumerator TransitionToState(GameStateType newState)
    {
        // Optional: Add a delay before changing states (e.g., for transition effects)
        if (newState != GameStateType.MainMenu)
        {
            yield return new WaitForSecondsRealtime(delay);
        }
  
        currentState = newState;
        HandleStateChange();
    }

    private void HandleStateChange()
    {

        HideAllMenu();

        switch (currentState)
        {
            case GameStateType.MainMenu:
                // Handle main menu logic
                Time.timeScale = 0f; // Pause the game
                mainMenuUI.SetActive(true);
                break;
            case GameStateType.Playing:
                // Handle playing logic
                Time.timeScale = 1f; // Resume the game
                playingUI.SetActive(true);
                break;
            case GameStateType.Paused:
                // Handle paused logic
                Time.timeScale = 0f; // Pause the game
                pauseMenuUI.SetActive(true);
                InputSystem.EnableDevice(Mouse.current);
                break;
        }
    }

    private void HideAllMenu()
    {
        mainMenuUI.SetActive(false);
        playingUI.SetActive(false);
        pauseMenuUI.SetActive(false);
    }

}
