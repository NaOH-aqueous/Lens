using UnityEngine;
using UnityEngine.InputSystem;
using Ink.Runtime;
using UnityEditor.SearchService;

public class DialogueTrigger : MonoBehaviour
{
    //set it to compiled json file
    public TextAsset inkAsset;

    [SerializeField] private GameObject indication;

    private bool playerInRange = false;

    private void Start()
    {
        indication.SetActive(false);    
    }

    private void Update()
    {
        if (!playerInRange)
        {
            return;
        }
        if( !DialogueManager.Instance.CheckDialoguePlaying())
        {
            if (InputManager.Instance.IsInteractPressed())
            {
                InputManager.Instance.RegisterSubmitPressed();
                DialogueManager.Instance.NewStory(inkAsset);
                Debug.Log("Current story has been set to " + inkAsset.name);
            }
        }
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("player is nearby");
            playerInRange = true;
            indication.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            indication.SetActive(false);
        }

    }


}
