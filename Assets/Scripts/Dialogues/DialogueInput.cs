using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueInput : MonoBehaviour
{
    //Make it a singleton object
    public static DialogueInput Instance { get; private set; }

    private InputAction interactAction;
    private InputAction SubmitAction;
    private InputAction CancelAction;

    private bool InteractPressed;
    private bool SubmitPressed;
    private bool CancelPressed;

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
        CancelAction = InputSystem.actions. FindAction("Cancel");


        interactAction.performed += OnInteract;
        SubmitAction.performed += OnConfirm;
        CancelAction.performed += OnCancel;
    }

    void OnEnable()
    {
        interactAction.Enable();
        SubmitAction.Enable();
        CancelAction.Enable();

        // To disable the mouse:
        InputSystem.DisableDevice(Mouse.current);
    }

    void OnDisable()
    {
        interactAction.Disable();
        SubmitAction.Disable();
        CancelAction.Disable();

        // To enable the mouse:
        //InputSystem.EnableDevice(Mouse.current);
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

    private void OnCancel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            CancelPressed = true;
        }
        else
        {
            CancelPressed = false;
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

    public bool IsCancelPressed()
    {
        bool result = CancelPressed;
        CancelPressed = false;
        return result;
    }

    public void RegisterSubmitPressed()
    {
        SubmitPressed = false;
    }

    public void RegisterInteractPressed()
    {
        InteractPressed = false;
    }
}
