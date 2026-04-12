using UnityEngine;
using UnityEngine.InputSystem;
using Ink.Runtime;
using UnityEditor.SearchService;
using System.Collections.Generic;
using Unity.VisualScripting;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue Controls")] 
    public TextAsset inkAsset;
    private PlayerController player;
    [SerializeField] private GameObject indication;
    private bool playerInRange = false;

    [Header("Audios")]
    [SerializeField] private List<TriggerSounds> triggerSounds = new List<TriggerSounds>();
    private AudioSource audioSource;
    

    private void Start()
    {
        indication.SetActive(false);
        player = GameObject.FindAnyObjectByType<PlayerController>();
        if(player == null)
        {
            Debug.Log("no player found in the scene");
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
        if (!DialogueManager.Instance.CheckDialoguePlaying())
        {
            StartDialogue();
        }
        else
        {
            PlayTriggerSound();
        }
    }

    private void StartDialogue()
    {
        int layerIndex = gameObject.layer;
        string layerName = LayerMask.LayerToName(gameObject.layer);
        if (InputManager.Instance.IsInteractPressed() &&
            player.CheckInteract(layerName))
        {
            InputManager.Instance.RegisterSubmitPressed();
            DialogueManager.Instance.NewStory(inkAsset);
            Debug.Log("Current story has been set to " + inkAsset.name);
        }
    }

    private void PlayTriggerSound()
    {
        // Play the trigger sound
        foreach(TriggerSounds triggerSound in triggerSounds)
        {
            if (triggerSound != null && 
                !string.IsNullOrEmpty(triggerSound.triggerSoundName))
            {
                DialogueManager.Instance.PlaySound(triggerSound.triggerSound, triggerSound.triggerSoundName);
            }
            else
            {
                Debug.Log("Triggersound hasn't been defined");
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
