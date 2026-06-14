using MaskTransitions;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenuController : MonoBehaviour
{
    [Header("Menu Screens")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject mainMenu;

    [Header("Slider")]
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private float sliderSmoothSpeed = 0.15f;
    [SerializeField] private float finalHoldSeconds = 0.1f;
    [SerializeField] private float totalTransitionTime = 2f;

    public void OnStartButton()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.StartGame(); 
        }
        else
        {
            StartCoroutine(LoadSceneAsync("Home"));
        }
    }

    public void OnQuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void Start()
    {
        InputSystem.EnableDevice(Mouse.current);
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        if (loadingScreen != null)
            loadingScreen.SetActive(true);

        if (loadingSlider != null)
            loadingSlider.value = 0f;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        
        if (operation == null)
        {
            Debug.LogError($"Failed to load scene: {sceneName}");
            yield break;
        }
        
        operation.allowSceneActivation = false; 

        float displayedProgress = loadingSlider != null ? loadingSlider.value : 0f;

        while (!operation.isDone)
        {
            float targetProgress = Mathf.Clamp01(operation.progress / 0.9f);

            displayedProgress = Mathf.MoveTowards(displayedProgress, targetProgress, sliderSmoothSpeed * Time.unscaledDeltaTime);

            if (loadingSlider != null)
                loadingSlider.value = displayedProgress;

            if (operation.progress >= 0.9f)
            {
                displayedProgress = Mathf.MoveTowards(displayedProgress, 1f, sliderSmoothSpeed * Time.unscaledDeltaTime);

                if (loadingSlider != null)
                    loadingSlider.value = displayedProgress;


                if (displayedProgress >= 0.999f)
                {
                    yield return new WaitForSecondsRealtime(finalHoldSeconds);
                    yield return null;
                    TransitionManager.Instance.PlayEndHalfTransition(totalTransitionTime / 2);
                    operation.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }
}
