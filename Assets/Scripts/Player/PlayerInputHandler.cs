using UnityEngine;
using UnityEngine.InputSystem;
using Core.Events;

namespace Player.Input
{
    [DefaultExecutionOrder(-1)] // Ensure input processing happens before other scripts
    public class PlayerInputHandler : MonoBehaviour
    {
        private static PlayerInputHandler instance;
        public static PlayerInputHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    //instance = FindObjectOfType<PlayerInputHandler>(); // Original line
                    instance = FindAnyObjectByType<PlayerInputHandler>(); // Modified line
                    if (instance == null)
                    {
                        Debug.LogError("No PlayerInputHandler found in scene!");
                    }
                }
                return instance;
            }
        }

        [SerializeField] private InputActionAsset playerControls;
        [SerializeField] private string actionMapName = "2DPlayer";

        private InputActionMap actionMap;
        private InputAction moveAction;
        private InputAction jumpAction;
        private InputAction sprintAction;
        private InputAction dashAction;
        private InputAction attackAction;
        private InputAction specialAction;

        private Vector2 moveInput;
        private bool jumpTriggered;
        private float sprintValue;

        // Events
        public event System.Action<Vector2> OnMoveInputChanged;
        public event System.Action OnJumpInputChanged;
        public event System.Action OnSprintInputChanged;
        public event System.Action OnDashInputChanged;
        public event System.Action OnAttackInputChanged;
        public event System.Action OnSpecialInputChanged;

        // Properties with private setters
        public Vector2 MoveInput => moveInput;
        public bool JumpTriggered { get => jumpTriggered; set => jumpTriggered = value; }
        public float SprintValue => sprintValue;

        private void Awake()
        {
            InitializeInputActions();
        }

        private void InitializeInputActions()
        {
            actionMap = playerControls.FindActionMap(actionMapName);

            moveAction = actionMap.FindAction("Move");
            jumpAction = actionMap.FindAction("Jump");
            sprintAction = actionMap.FindAction("Sprint");
            dashAction = actionMap.FindAction("Dash");
            attackAction = actionMap.FindAction("Attack");
            specialAction = actionMap.FindAction("SpecialPower");

            RegisterInputCallbacks();
        }

        private void RegisterInputCallbacks()
        {
            moveAction.performed += ctx =>
            {
                moveInput = ctx.ReadValue<Vector2>();
                OnMoveInputChanged?.Invoke(moveInput);
            };
            moveAction.canceled += _ =>
            {
                moveInput = Vector2.zero;
                OnMoveInputChanged?.Invoke(moveInput);
            };

            jumpAction.performed += _ =>
            {
                jumpTriggered = true;
                OnJumpInputChanged?.Invoke();
            };
            jumpAction.canceled += _ => jumpTriggered = false;

            sprintAction.performed += ctx =>
            {
                sprintValue = ctx.ReadValue<float>();
                OnSprintInputChanged?.Invoke();
            };
            sprintAction.canceled += _ =>
            {
                sprintValue = 0f;
                OnSprintInputChanged?.Invoke();
            };

            dashAction.performed += _ => OnDashInputChanged?.Invoke();
            attackAction.performed += _ => OnAttackInputChanged?.Invoke();
            specialAction.performed += _ => OnSpecialInputChanged?.Invoke();
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
            actionMap.Enable();
        }

        private void DisableInputActions()
        {
            actionMap.Disable();
        }
    }
}
