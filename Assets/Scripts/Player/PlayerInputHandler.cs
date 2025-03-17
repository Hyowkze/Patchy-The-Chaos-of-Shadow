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

    [SerializeField] private string attackActionName = "Attack";
    [SerializeField] private string specialActionName = "SpecialPower";

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction dashAction;
    private InputAction attackAction;
    private InputAction specialAction;

    public Vector2 MoveInput { get; private set; }
    internal bool JumpTriggered { get; set; }
    public float SprintValue { get; private set; }
    internal bool DashTriggered { get; set; }
    internal bool AttackTriggered { get; set; }
    internal bool SpecialTriggered { get; set; }

    public static PlayerInputHandler Instance { get; private set; }

    public event System.Action<Vector2> OnMoveInputChanged;
    public event System.Action OnJumpInputChanged;
    public event System.Action OnDashInputChanged; // Added
    public event System.Action OnAttackInputChanged; // Added
    public event System.Action OnSpecialInputChanged; // Added

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
        attackAction = actionMap.FindAction(attackActionName, true);
        specialAction = actionMap.FindAction(specialActionName, true);

        RegisterInputActions();
    }

    private void RegisterInputActions()
    {
        // Subscribe to the "performed" and "canceled" events of each input action
        moveAction.performed += ctx => {
            MoveInput = ctx.ReadValue<Vector2>();
            OnMoveInputChanged?.Invoke(MoveInput);
        };
        moveAction.canceled += _ => {
            MoveInput = Vector2.zero;
            OnMoveInputChanged?.Invoke(MoveInput);
        };

        jumpAction.performed += ctx => {
            JumpTriggered = true;
            OnJumpInputChanged?.Invoke();
            Debug.Log("Jump Input Performed");
        };
        jumpAction.canceled += OnJumpCanceled;

        sprintAction.performed += ctx => SprintValue = ctx.ReadValue<float>();
        sprintAction.canceled += _ => SprintValue = 0f; // Reset sprint value

        dashAction.performed += _ => {
            DashTriggered = true;
            OnDashInputChanged?.Invoke(); // Trigger the event
        };
        dashAction.canceled += _ => DashTriggered = false;

        attackAction.performed += _ => {
            AttackTriggered = true;
            OnAttackInputChanged?.Invoke(); // Trigger the event
        };
        attackAction.canceled += _ => AttackTriggered = false;

        specialAction.performed += _ => {
            SpecialTriggered = true;
            OnSpecialInputChanged?.Invoke(); // Trigger the event
        };
        specialAction.canceled += _ => SpecialTriggered = false;
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
        attackAction.Enable();
        specialAction.Enable();
    }

    private void DisableInputActions()
    {
        moveAction.Disable();
        jumpAction.Disable();
        sprintAction.Disable();
        dashAction.Disable();
        attackAction.Disable();
        specialAction.Disable();
    }
}
