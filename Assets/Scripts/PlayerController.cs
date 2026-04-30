using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Player Controls")]
    public float moveSpeed = 4f;
    public float footstepInterval;
    public AudioClip footStep;

    //private variables
    private Rigidbody2D playerRB;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private DialogueManager m_dialogueManager;
    private bool isInDialogue = false;
    private Vector2 moveValue;

    //Animator Componenets
    private Animator animator;
    Vector2 moveDirection = new Vector2(1, 0);

    //Sounds
    private AudioSource footstepAudio;
    private float stepTimer;

    //GameManager
    private GameManager m_gameManager;
    private GameStateType gameState;

    private void Start()
    {
        Init();
    }
    private void Update()
    {
        PauseGame();
    }

    private void OnEnable()
    {
        DialogueManager.Instance.OnDialogueStatusChanged += SetPlayerControl;
    }

    private void OnDisable()
    {
        DialogueManager.Instance.OnDialogueStatusChanged -= SetPlayerControl;
    }

    private void FixedUpdate()
    {
        if (!isInDialogue)
        {
            moveValue = moveAction.ReadValue<Vector2>();
            MovePlayer();
            FootStep();
        }
        else
        {
            StopMovementAnimation();
        }
    }

    private void Init()
    {
        playerRB = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
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

    public void SetPlayerControl(bool DisableControl)
    {
        if (DisableControl)
        {
            playerInput.actions["Move"].Disable();
            playerInput.actions["Interact"].Disable();
            playerInput.actions["Inventory"].Disable();
        }
        else
        {
            playerInput.actions["Move"].Enable();
            playerInput.actions["Interact"].Enable();
            playerInput.actions["Inventory"].Enable();
        }
    }

    private void FootStep() //play footstep audio when player is walking
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

    private void StopMovementAnimation() //set movement of animation to zero
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", 0);
        }
    } 

    private void PauseGame()  //pause the game according to user inputs
    {
        if (InputManager.Instance.IsCancelPressed())
        {
            // Show Pause Menu UI
            m_gameManager.ChangeToPaused();
        }
    }

    private void MovePlayer() //move the player according to user inputs
    {
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
        Vector2 currentPos = playerRB.position;
        Vector2 targetPos = currentPos + moveSpeed * moveValue.normalized * Time.deltaTime;
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
            footstepAudio.PlayOneShot(footStep);
        }
    }  //play the audioclip of footstep

    //check if the current interaction raycast has touches any interactable
    //objects
    public bool CheckInteract(string layerName)
    {
        //shoot a raycast from the player to find if any furnitures in range
        RaycastHit2D hit = Physics2D.Raycast(playerRB.position +
            Vector2.up * 0.4f, moveDirection, 3f, LayerMask.GetMask(layerName));

        //Debug.DrawRay(playerRB.position + Vector2.up * 0.4f, moveDirection, Color.green);

        if (hit.collider != null)
        {
            Debug.Log("Raycast has hit the object " + hit.collider.gameObject);
            return true;
        }
        return false;
    }
}
