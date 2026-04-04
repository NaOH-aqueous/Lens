using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    //Make it a singleton object
    public static InputManager Instance { get; private set; }

    private InputAction interactAction;
    private InputAction SubmitAction;
    private InputAction CancelAction;
    private InputAction UseAction;
    private InputAction InventoryAction;

    private bool InteractPressed;
    private bool SubmitPressed;
    private bool CancelPressed;
    private bool UsePressed;
    private bool InventoryPressed;

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
        CancelAction = InputSystem.actions.FindAction("Cancel");
        UseAction = InputSystem.actions.FindAction("Use");
        InventoryAction = InputSystem.actions.FindAction("Inventory");

        interactAction.performed += OnInteract;
        SubmitAction.performed += OnConfirm;
        CancelAction.performed += OnCancel;
        UseAction.performed += OnUse;
        InventoryAction.performed += OnInventory;
    }

    void OnEnable()
    {
        interactAction.Enable();
        SubmitAction.Enable();
        CancelAction.Enable();
        UseAction.Enable();
        InventoryAction.Enable();

        // To disable the mouse:
        InputSystem.DisableDevice(Mouse.current);
    }

    void OnDisable()
    {
        interactAction.Disable();
        SubmitAction.Disable();
        CancelAction.Disable();
        UseAction.Disable();
        InventoryAction.Disable();

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
    
    private void OnUse(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            UsePressed = true;
        }
        else
        {
            UsePressed = false;
        }
    }

    private void OnInventory(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            InventoryPressed = true;
        }
        else
        {
            InventoryPressed = false;
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

    public bool isUsePressed()
    {
        bool result = UsePressed;
        CancelPressed = false;
        return result;
    }

    public bool isInventoryPressed()
    {
        bool result = InventoryPressed;
        InventoryPressed = false;
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
