using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        // Disable player input
        InputRouter.Instance.PushLayer(InputLayer.Cutscene);

        yield return SceneManager.LoadSceneAsync(sceneName);

        yield return null;

        yield return new WaitUntil(() => SceneInitializer.SceneReady);

        DialogueManager.Instance.CancelDialogueImmediately();

        InputRouter.Instance.PopLayer(InputLayer.Cutscene);
    }
}
