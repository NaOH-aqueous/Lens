using UnityEngine;

public class Mirror : MonoBehaviour
{
    private DialogueTrigger dialogue;
    
    public string read_times = "read_times";
    public string dialogueID = "mirror";

    private void Awake()
    {
        dialogue = gameObject.GetComponentInChildren<DialogueTrigger>();
    }

    private void Update()
    {
    }
}
