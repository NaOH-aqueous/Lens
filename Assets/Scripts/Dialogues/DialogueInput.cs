using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueInput : MonoBehaviour
{
    //Make it a singleton object
    public static DialogueInput Instance { get; private set; }

    private InputAction interactAction;
    private InputAction SubmitAction;

    private bool InteractPressed;
    private bool SubmitPressed;

    void Awake()
    {
        //make it a singleton gameobject
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        interactAction = InputSystem.actions.FindAction("Interact");
        SubmitAction = InputSystem.actions.FindAction("Submit");

        interactAction.performed += OnInteract;
        SubmitAction.performed += OnConfirm;
    }

    void OnEnable()
    {
        interactAction.Enable();
        SubmitAction.Enable();

        // To disable the mouse:
        InputSystem.DisableDevice(Mouse.current);
    }

    void OnDisable()
    {
        interactAction.Disable();
        SubmitAction.Disable();

        // To enable the mouse:
        InputSystem.EnableDevice(Mouse.current);
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            InteractPressed = true;
        }
        else
        {
            InteractPressed = false;
        }
    }

    private void OnConfirm(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SubmitPressed = true;
        }
        else
        {
            SubmitPressed = false;
        }
    }

    public bool IsInteractPressed()
    {
        bool result = InteractPressed; 
        InteractPressed = false;
        return result;
    }

    public bool IsSubmitPressed()
    {
        bool result = SubmitPressed;
        SubmitPressed = false;
        return result;
    }

    public void RegisterSubmitPressed()
    {
        SubmitPressed = false;
    }
}
