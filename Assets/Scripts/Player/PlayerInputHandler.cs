using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace Player.Input
{
    public class PlayerInputHandler : MonoBehaviour
    {
        public static PlayerInputHandler Instance { get; private set; }

        public event Action<Vector2> OnMoveInputChanged;
        public event Action OnJumpInputChanged;
        public event Action OnSprintInputChanged;
        public event Action OnDashInputChanged;
        public event Action OnAttackInputChanged;
        public event Action OnSpecialPowerInputChanged; // Renamed from OnSpecialInputChanged

        public Vector2 MoveInput { get; set; }
        public float SprintValue { get; private set; } // Change to private set

        private PlayerInputActions playerInputActions; // Now it will be found

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            playerInputActions = new PlayerInputActions(); // Now it will be found
            playerInputActions.Player.Enable();

            playerInputActions.Player.Move.performed += OnMove;
            playerInputActions.Player.Move.canceled += OnMove;
            playerInputActions.Player.Jump.performed += OnJump;
            playerInputActions.Player.Sprint.performed += OnSprint;
            playerInputActions.Player.Dash.performed += OnDash;
            playerInputActions.Player.Attack.performed += OnAttack;
            playerInputActions.Player.SpecialPower.performed += OnSpecialPower; // Updated name
        }

        private void OnDestroy()
        {
            playerInputActions.Player.Move.performed -= OnMove;
            playerInputActions.Player.Move.canceled -= OnMove;
            playerInputActions.Player.Jump.performed -= OnJump;
            playerInputActions.Player.Sprint.performed -= OnSprint;
            playerInputActions.Player.Dash.performed -= OnDash;
            playerInputActions.Player.Attack.performed -= OnAttack;
            playerInputActions.Player.SpecialPower.performed -= OnSpecialPower;
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
            OnMoveInputChanged?.Invoke(MoveInput);
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            OnJumpInputChanged?.Invoke();
        }

        private void OnSprint(InputAction.CallbackContext context)
        {
            SprintValue = SprintValue == 0 ? 1 : 0; // Toggle sprint value
            OnSprintInputChanged?.Invoke();
        }

        private void OnDash(InputAction.CallbackContext context)
        {
            OnDashInputChanged?.Invoke();
        }

        private void OnAttack(InputAction.CallbackContext context)
        {
            OnAttackInputChanged?.Invoke();
        }

        private void OnSpecialPower(InputAction.CallbackContext context) // Renamed from OnSpecial
        {
            OnSpecialPowerInputChanged?.Invoke();
        }

        public bool IsActionTriggered(string actionName)
        {
            InputAction action = playerInputActions.FindAction(actionName);
            return action != null && action.triggered;
        }
    }
}
