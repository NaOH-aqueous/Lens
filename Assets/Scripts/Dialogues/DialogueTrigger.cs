using UnityEngine;
using UnityEngine.InputSystem;
using Ink.Runtime;
using UnityEditor.SearchService;

public class DialogueTrigger : MonoBehaviour
{
    //set it to compiled json file
    public TextAsset inkAsset;
    private PlayerController player;

    [SerializeField] private GameObject indication;

    private bool playerInRange = false;

    //Audio Sounds
    public AudioClip triggerSound;
    private AudioSource audioSource;


    private void Start()
    {
        indication.SetActive(false);
        player = GameObject.FindAnyObjectByType<PlayerController>();
        if(player == null)
        {
            Debug.Log("no setted player in the scene");
        }

        audioSource = GetComponent<AudioSource>();
        // If there is no AudioSource component, add one
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        if (!playerInRange)
        {
            return;
        }
        if( !DialogueManager.Instance.CheckDialoguePlaying())
        {
            if (InputManager.Instance.IsInteractPressed() &&
                player.CheckInteract())
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
