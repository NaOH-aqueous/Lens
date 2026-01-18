using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //public variables
    public float moveSpeed = 4f;

    //private variables
    private Rigidbody2D playerRB;
    private InputAction moveAction;
    private DialogueManager m_dialogueManager;

    private void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        moveAction = InputSystem.actions.FindAction("Move");

        m_dialogueManager = GameObject.FindAnyObjectByType<DialogueManager>();
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

    private void MovePlayer()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();

        //move the rigidbody of the player
        Vector2 currentPos = playerRB.transform.position;
        Vector2 targetPos = currentPos += moveSpeed * moveValue * Time.deltaTime;

        playerRB.MovePosition(targetPos);
    }
}
