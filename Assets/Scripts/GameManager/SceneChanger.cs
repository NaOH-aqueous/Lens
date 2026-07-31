using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string sceneToLoad;
    public string exitPointName = "ExitPoint";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Player entered the scene changer trigger.");
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Player collided with the scene changer. Loading scene: " + sceneToLoad);
            SceneTransitionManager.Instance.LoadScene(sceneToLoad, exitPointName);
        }
    }
}
