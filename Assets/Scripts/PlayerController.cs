using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    //public variables
    public float moveSpeed = 4f;
    public float footstepInterval;

    //private variables
    private Rigidbody2D playerRB;
    private InputAction moveAction;
    private DialogueManager m_dialogueManager;

    //Animator Componenets
    private Animator animator;
    Vector2 moveDirection = new Vector2(1, 0);

    //Sounds
    private AudioSource footstepAudio;
    private float stepTimer;

    //Inventory
    // Simple inventory array with 10 slots
    public List<Item> inventory;


    private Item current_item;

    private void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        moveAction = InputSystem.actions.FindAction("Move");

        m_dialogueManager = GameObject.FindAnyObjectByType<DialogueManager>();

        //Footsteps Audio
        footstepAudio = GetComponent<AudioSource>();
        if (footstepAudio == null)
        {
            footstepAudio = gameObject.AddComponent<AudioSource>();
            footstepAudio.playOnAwake = false;
        }

    }
    private void Update()
    {
        if (CheckIfPlayerCanMove())
        {
            // Update the step timer
            stepTimer += Time.deltaTime;

            // Check if the player is moving and if it's time to play the footstep sound
            if (moveAction.ReadValue<Vector2>().magnitude > 0.1f
                && stepTimer >= footstepInterval)
            {
                PlayFootstepAudio();
                // Reset the timer after playing the sound
                stepTimer = 0f;
            }
        }
        /* Temporary key to display inventory contents in the console
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            DisplayInventory(new InputAction.CallbackContext());
        }*/

        RemoveItem();
        DisplayInventory();
    }

    private void FixedUpdate()
    {
        if (CheckIfPlayerCanMove())
        {
            MovePlayer();
        }
    }

    //return TRUE when the dialogue isn't playing or there's no dialogue in the scene
    private bool CheckIfPlayerCanMove()
    {
        if (m_dialogueManager != null) 
        {
            if (m_dialogueManager.CheckDialoguePlaying())
            {
                return false;
            }
        }
        return true;
    }

    private void DisplayInventory()
    {
        if (DialogueInput.Instance.isInventoryPressed())
        {
            Inventory.Instance.GetInventoryContent();
        }
    }

    private void MovePlayer()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();

        //set the animator values according to player movement
        if (!Mathf.Approximately(moveValue.x, 0.0f) || !Mathf.Approximately(moveValue.y, 0.0f))
        {
            moveDirection.Set(moveValue.x, moveValue.y);
            moveDirection.Normalize();
        }
        animator.SetFloat("MoveX", moveDirection.x);
        animator.SetFloat("MoveY", moveDirection.y);
        animator.SetFloat("Speed", moveValue.magnitude);

        //move the rigidbody of the player
        Vector2 currentPos = playerRB.transform.position;
        Vector2 targetPos = currentPos += moveSpeed * moveValue * Time.deltaTime;
        playerRB.MovePosition(targetPos);
    }

    void PlayFootstepAudio()
    {
        if (footstepAudio == null)
        {
            Debug.Log("No AudioSource found for footstep sounds.");
            return; // No audio source available
        }

        // Check if there's a clip assigned to the audio source
        if (footstepAudio.clip != null)
        {
            footstepAudio.PlayOneShot(footstepAudio.clip);
        }
    }

    private void PickUpItem()
    {
        //Inventory
        if (DialogueInput.Instance.IsInteractPressed())
        {
            Item currentItem = Inventory.Instance.GetCurrentItem();
            //pick it up if it exists
            if (currentItem != null)
            {
                Inventory.Instance.AddItem(currentItem, currentItem.itemID);
            }
            //_itemDatabase.AddItem(0, this);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            PickUpItem();
        }
    }

    private void RemoveItem()
    {
        if (DialogueInput.Instance.IsCancelPressed())
        {
            // request item by ID and remove it from the inventory
            Inventory.Instance.RemoveItem(0);
        }
    }
}
