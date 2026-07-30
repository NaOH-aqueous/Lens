using UnityEngine;

public class SceneGlobals : MonoBehaviour
{
    [SerializeField]
    private TextAsset globalsJSON;

    private void Start()
    {
        DialogueManager.Instance.SwitchGlobals(globalsJSON);
    }
}