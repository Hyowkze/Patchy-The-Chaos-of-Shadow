using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Action Map and Action Names")]
    [SerializeField] private string actionMapName = "2DPlayer";
    [SerializeField] private string moveActionName = "Move";
    [SerializeField] private string jumpActionName = "Jump";
    [SerializeField] private string sprintActionName = "Sprint";
    [SerializeField] private string dashActionName = "Dash";

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction dashAction;

    public Vector2 MoveInput { get; private set; }
    internal bool JumpTriggered { get; set; }
    public float SprintValue { get; private set; }
    internal bool DashTriggered { get; set; }

    public static PlayerInputHandler Instance { get; private set; }

    private void Awake()
    {
        // Singleton pattern implementation.
        if (Instance == null)
        {
            Instance = this;
            //Check if the gameobject is a root object.
            if (transform.parent != null)
            {
                //If its not a root gameobject, we detach it.
                transform.SetParent(null);
            }
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return; // Exit, so that no other code executes.
        }

        // Get the action map
        InputActionMap actionMap = playerControls.FindActionMap(actionMapName, true);

        // Find each action and assign it
        moveAction = actionMap.FindAction(moveActionName, true);
        jumpAction = actionMap.FindAction(jumpActionName, true);
        sprintAction = actionMap.FindAction(sprintActionName, true);
        dashAction = actionMap.FindAction(dashActionName, true);

        RegisterInputActions();
    }

    private void RegisterInputActions()
    {
        // Subscribe to the "performed" and "canceled" events of each input action
        moveAction.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        moveAction.canceled += _ => MoveInput = Vector2.zero; //Using _ to replace ctx

        jumpAction.performed += OnJumpPerformed;
        jumpAction.canceled += OnJumpCanceled;

        sprintAction.performed += ctx => SprintValue = ctx.ReadValue<float>();
        sprintAction.canceled += _ => SprintValue = 0f; //Using _ to replace ctx

        dashAction.performed += _ => DashTriggered = true;
        dashAction.canceled += _ => DashTriggered = false;
    }

    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        JumpTriggered = true;
        Debug.Log("Jump Input Performed");
    }

    private void OnJumpCanceled(InputAction.CallbackContext ctx)
    {
        JumpTriggered = false;
        Debug.Log("Jump Input Canceled");
    }

    private void OnEnable()
    {
        EnableInputActions();
    }

    private void OnDisable()
    {
        DisableInputActions();
    }

    private void EnableInputActions()
    {
        moveAction.Enable();
        jumpAction.Enable();
        sprintAction.Enable();
        dashAction.Enable();
    }

    private void DisableInputActions()
    {
        moveAction.Disable();
        jumpAction.Disable();
        sprintAction.Disable();
        dashAction.Disable();
    }
}
