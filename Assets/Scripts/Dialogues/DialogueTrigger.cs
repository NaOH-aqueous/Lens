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

    //Audio Sounds
    public AudioClip triggerSound;
    private AudioSource audioSource;

    private void Start()
    {
        indication.SetActive(false);    

        audioSource = GetComponent<AudioSource>();
        // If there is no AudioSource component, add one
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
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

                // Play the trigger sound
                if (triggerSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(triggerSound);
                }

                DialogueManager.Instance.NewStory(inkAsset);
                Debug.Log("Current story has been set to " + inkAsset.name);
            }
        }
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("player is nearby");
            playerInRange = true;
            indication.SetActive(true);
            InputManager.Instance.RegisterInteractPressed();
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
