using System.Collections;
using UnityEngine;
public class SceneInitializer : MonoBehaviour
{
    public static bool SceneReady { get; private set; }
    public FlowController flowController;

    IEnumerator Start()
    {
        SceneReady = false;

        // Wait one frame so every Awake() has finished.
        yield return null;

        // Wait another frame so every Start() has finished.
        yield return null;

        // Restore dialogues
        while (!DialogueManager.Instance.IsReady)
        {
            yield return null;
        }

        // Restore save data
        // Restore inventory
        while (!InventoryManager.Instance.IsReady)
        {
            yield return null;
        }
        // Restore dialogue variables
        while (!DialogueManager.Instance.SaveComplete)
        {
            yield return null;
        }

        while (!flowController.IsReady)
        {
            yield return null;
        }

        DialogueManager.Instance.CancelDialogueImmediately();

        Debug.Log("scene loading complete");

        SceneReady = true;
    }
}
