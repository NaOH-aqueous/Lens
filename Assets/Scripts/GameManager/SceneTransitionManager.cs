using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    public Animator fadeAnim;
    public float fadeDuration = 0.5f;

    private static string pendingExitPointName = "ExitPoint";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void LoadScene(string sceneName, string exitPointName = "ExitPoint")
    {
        pendingExitPointName = exitPointName;
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {

        Debug.Log("LoadSceneRoutine ¿ªÊ¼");

        fadeAnim.Play("FadeTo");
        yield return new WaitForSeconds(fadeDuration);

        // Disable player input
        InputRouter.Instance.PushLayer(InputLayer.Cutscene);

        // Load the new scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Wait for the scene to be ready
        yield return null;

        GameObject exitPoint = GameObject.Find(pendingExitPointName);

        if (exitPoint != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                player.transform.position = exitPoint.transform.position;
                Debug.Log($"Player moved to exit point: {pendingExitPointName}");
            }
            else
            {
                Debug.LogWarning("Player object not found in the scene.");
            }
        }
        else
        {
            Debug.LogWarning($"Exit point '{pendingExitPointName}' not found in the scene.");
        }

        //yield return SceneManager.LoadSceneAsync(sceneName);

        //yield return null;

        //yield return new WaitUntil(() => SceneInitializer.SceneReady);

        DialogueManager.Instance.CancelDialogueImmediately();

        InputRouter.Instance.PopLayer(InputLayer.Cutscene);

        fadeAnim.Play("FadeFrom");

    }

}
