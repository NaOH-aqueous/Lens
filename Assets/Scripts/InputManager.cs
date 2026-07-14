using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    //Make it a singleton object
    public static InputManager Instance { get; private set; }

    private InputAction interactAction;
    private InputAction SubmitAction;
    private InputAction CancelAction;
    private InputAction InventoryAction;

    private bool InteractPressed;
    private bool SubmitPressed;
    private bool CancelPressed;
    private bool InventoryPressed;

    // Event-based API (subscribers will be notified when the input is performed)
    public event Action InteractPerformed;
    public event Action SubmitPerformed;
    public event Action CancelPerformed;
    public event Action InventoryPerformed;

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
        InventoryAction = InputSystem.actions.FindAction("Inventory");

        if (interactAction != null)
            interactAction.performed += OnInteract;
        else
            Debug.LogWarning("InputManager: 'Interact' action not found.");

        if (SubmitAction != null)
            SubmitAction.performed += OnConfirm;
        else
            Debug.LogWarning("InputManager: 'Submit' action not found.");

        if (CancelAction != null)
            CancelAction.performed += OnCancel;
        else
            Debug.LogWarning("InputManager: 'Cancel' action not found.");

        if (InventoryAction != null)
            InventoryAction.performed += OnInventory;
        else
            Debug.LogWarning("InputManager: 'Inventory' action not found.");
    }

    void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks / dangling delegates
        if (interactAction != null)
            interactAction.performed -= OnInteract;

        if (SubmitAction != null)
            SubmitAction.performed -= OnConfirm;

        if (CancelAction != null)
            CancelAction.performed -= OnCancel;

        if (InventoryAction != null)
            InventoryAction.performed -= OnInventory;
    }

    void OnEnable()
    {
        if (interactAction != null)
            interactAction.Enable();
        if (SubmitAction != null)
            SubmitAction.Enable();
        if (CancelAction != null)
            CancelAction.Enable();
        if (InventoryAction != null)
            InventoryAction.Enable();

        // To disable the mouse:
        InputSystem.DisableDevice(Mouse.current);
    }

    void OnDisable()
    {
        if (interactAction != null)
            interactAction.Disable();
        if (SubmitAction != null)
            SubmitAction.Disable();
        if (CancelAction != null)
            CancelAction.Disable();
        if (InventoryAction != null)
            InventoryAction.Disable();

        // To enable the mouse:
        //InputSystem.EnableDevice(Mouse.current);
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (!InputRouter.Instance.AllowsGameplayInput())
            return;

        if (context.performed)
        {
            InteractPressed = true;
            InteractPerformed?.Invoke();
        }
        else
        {
            InteractPressed = false;
        }
    }

    private void OnConfirm(InputAction.CallbackContext context)
    {       
        if (InputRouter.Instance.BlocksEverything())
            return;

        if (context.performed)
        {
            SubmitPressed = true;
            SubmitPerformed?.Invoke();
        }
        else
        {
            SubmitPressed = false;
        }
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        if (InputRouter.Instance.BlocksEverything())
            return;

        if (context.performed)
        {
            CancelPressed = true;
            CancelPerformed?.Invoke();
        }
        else
        {
            CancelPressed = false;
        }
    }

    private void OnInventory(InputAction.CallbackContext context)
    {
        if (InputRouter.Instance.BlocksEverything())
            return;
        if (!InputRouter.Instance.AllowsInventoryInput())
            return;

        if (context.performed)
        {
            InventoryPressed = true;
            InventoryPerformed?.Invoke();
        }
        else
        {
            InventoryPressed = false;
        }
    }

    // Backwards-compatible polling API retained (still clears the flag when read)
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

    public bool IsInventoryPressed()
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

    public void RegisterCancelPressed()
    {
        CancelPressed = false;
    }
}
