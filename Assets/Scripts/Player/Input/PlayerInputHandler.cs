using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace Player.InputSystem
{
    public class PlayerInputHandler : MonoBehaviour
    {
        public static PlayerInputHandler Instance { get; private set; }

        public event Action<Vector2> OnMoveInputChanged;
        public event Action OnJumpInputChanged;
        public event Action OnSprintInputChanged;
        public event Action OnDashInputChanged;
        public event Action OnAttackInputChanged;
        public event Action OnSpecialPowerInputChanged;

        public Vector2 MoveInput { get; private set; }
        public float SprintValue { get; private set; }

        private PlayerInputActions inputActions;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            inputActions = new PlayerInputActions();
            inputActions.Player.Enable();

            BindInputActions();
        }

        private void BindInputActions()
        {
            inputActions.Player.Move.performed += OnMove;
            inputActions.Player.Move.canceled += OnMove;
            inputActions.Player.Jump.performed += OnJump;
            inputActions.Player.Sprint.performed += OnSprint;
            inputActions.Player.Dash.performed += OnDash;
            inputActions.Player.Attack.performed += OnAttack;
            inputActions.Player.SpecialPower.performed += OnSpecialPower;
        }

        private void OnDestroy()
        {
            if (inputActions != null)
            {
                UnbindInputActions();
                inputActions.Dispose();
            }
        }

        private void UnbindInputActions()
        {
            inputActions.Player.Move.performed -= OnMove;
            inputActions.Player.Move.canceled -= OnMove;
            inputActions.Player.Jump.performed -= OnJump;
            inputActions.Player.Sprint.performed -= OnSprint;
            inputActions.Player.Dash.performed -= OnDash;
            inputActions.Player.Attack.performed -= OnAttack;
            inputActions.Player.SpecialPower.performed -= OnSpecialPower;
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
            OnMoveInputChanged?.Invoke(MoveInput);
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnJumpInputChanged?.Invoke();
            }
        }

        private void OnSprint(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                SprintValue = SprintValue == 0 ? 1 : 0;
                OnSprintInputChanged?.Invoke();
            }
        }

        private void OnDash(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnDashInputChanged?.Invoke();
            }
        }

        private void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnAttackInputChanged?.Invoke();
            }
        }

        private void OnSpecialPower(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnSpecialPowerInputChanged?.Invoke();
            }
        }

        public bool IsActionTriggered(string actionName)
        {
            InputAction action = inputActions.FindAction(actionName);
            return action != null && action.triggered;
        }
    }
}
