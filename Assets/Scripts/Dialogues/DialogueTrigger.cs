using UnityEngine;
using System.Collections.Generic;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue Controls")] 
    public TextAsset inkAsset;
    private PlayerController player;
    [SerializeField] private GameObject indication;
    private bool playerInRange = false;
    private float lastDialogueTime = -1f;
    [SerializeField] private float interactionCooldown = 0.2f;

    [Header("Audios")]
    [SerializeField] private List<TriggerSounds> triggerSounds = new List<TriggerSounds>();
    
    private void Start()
    {
        indication.SetActive(false);
        player = GameObject.FindAnyObjectByType<PlayerController>();
        if(player == null)
        {
            Debug.Log("no player found in the scene");
        }
    }

    private void Update()
    {
        if (GameManager.instance.CurrentState != GameStateType.Playing)
        {
            return;
        }

        if (!playerInRange)
        {
            // return directly if player is not around
            return;
        }
    }

    //start the dialogue attached to this trigger upon user input
    private void StartDialogue()
    {
        if (Time.time < lastDialogueTime + interactionCooldown)
            return;

        if (GameManager.instance.CurrentState != GameStateType.Playing)
        {
            return;
        }

        int layerIndex = gameObject.layer;
        string layerName = LayerMask.LayerToName(gameObject.layer);

        if (player.CheckInteract(layerName))
        {
            lastDialogueTime = Time.time;
            DialogueManager.Instance.NewStory(inkAsset);
            Debug.Log("Current story has been set to " + inkAsset.name);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //set the indication and bool true if player is nearby
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            indication.SetActive(true);

            // Subscribe to interact event when player enters range
            if (InputManager.Instance != null)
            {
                // defensive unsubscribe to avoid duplicate subscriptions
                InputManager.Instance.InteractPerformed -= OnInteractPerformed;
                InputManager.Instance.InteractPerformed += OnInteractPerformed;
            }
            else
            {
                Debug.LogWarning("DialogueTrigger: InputManager instance not found when subscribing to InteractPerformed.");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //set the indication and bool false if player is nearby
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            indication.SetActive(false);

            // Unsubscribe from interact event when player leaves range
            if (InputManager.Instance != null)
            {
                InputManager.Instance.InteractPerformed -= OnInteractPerformed;
            }
        }

    }

    private void OnDestroy()
    {
        // Ensure no dangling subscription
        if (InputManager.Instance != null)
        {
            InputManager.Instance.InteractPerformed -= OnInteractPerformed;
        }
    }

    // Event handler for input (replaces polling)
    private void OnInteractPerformed()
    {
        // Mirror previous guards used in Update
        if (GameManager.instance == null || GameManager.instance.CurrentState != GameStateType.Playing)
            return;

        if (!playerInRange)
            return;

        if (DialogueManager.Instance != null && DialogueManager.Instance.CheckDialoguePlaying())
            return;

        StartDialogue();
    }
}
