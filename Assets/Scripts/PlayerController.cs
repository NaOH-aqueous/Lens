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

    //GameManager
    private GameManager m_gameManager;
    private void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        moveAction = InputSystem.actions.FindAction("Move");

        m_dialogueManager = GameObject.FindAnyObjectByType<DialogueManager>();
        m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();


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

        if (InputManager.Instance.IsCancelPressed())
        {
            // Show Pause Menu UI
            m_gameManager.ChangeToPaused();
        }
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
                StopMovementAnimation();
                return false;
            }
        }
        return true;
    }

    // Stop the movement animation by setting the animator parameters to zero
    private void StopMovementAnimation()
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", 0);
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

    public bool CheckInteract()
    {
        //shoot a raycast from the player to find if any furnitures in range
        RaycastHit2D hit = Physics2D.Raycast(playerRB.position +
            Vector2.up * 0.4f, moveDirection, 3.5f, LayerMask.GetMask("Furniture"));
        Debug.DrawRay(playerRB.position + Vector2.down * 0.4f, moveDirection, Color.green);

        if (hit.collider != null)
        {
            Debug.Log("Raycast has hit the object " + hit.collider.gameObject);
            return true;
        }
        return false;
    }
}