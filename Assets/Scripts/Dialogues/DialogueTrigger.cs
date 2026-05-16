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
            //return directly if player is not around
            return;
        }

        if (!DialogueManager.Instance.CheckDialoguePlaying())
        {
            if (InputManager.Instance.IsInteractPressed())
            {
                StartDialogue();
            }
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
            InputManager.Instance.RegisterSubmitPressed();
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
            //Debug.Log("player is nearby");
            playerInRange = true;
            indication.SetActive(true);
            InputManager.Instance.RegisterInteractPressed();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //set the indication and bool false if player is nearby
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            indication.SetActive(false);
        }

    }
}
