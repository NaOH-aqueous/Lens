using UnityEngine;

public class SetSprites : MonoBehaviour
{
    [SerializeField] private string speaker = "speaker";
    private void Update()
    {
        if (DialogueManager.Instance.CheckDialoguePlaying())
        {
            string currentSpeaker = DialogueManager.Instance.GetSpeakerTag();
            if (currentSpeaker == speaker)
            {
                Debug.Log("current speaker is " + speaker);
            }
        }
    }
}
