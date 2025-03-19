using UnityEngine;
using Core.Player.Movement;
using Core.Interfaces;
using Player.Movement; // Added this using directive

namespace Core.Player.Movement
{
    [RequireComponent(typeof(PatchyMovement))]
    public class MovementStateMachine : MonoBehaviour
    {
        [SerializeField] private PatchyMovement movement;
        [SerializeField] private Animator animator; // Added for animation control
        [SerializeField] private MovementConfig config;

        private static readonly int AnimatorState = Animator.StringToHash("State");

        public enum MovementState
        {
            Idle,
            Walking,
            Jumping,
            Dashing,
            Sprinting
        }

        private IMovementState currentState;
        private MovementStateFactory factory;

        public MovementState CurrentState { get; private set; }

        private void Reset()
        {
            movement = GetComponent<PatchyMovement>();
        }

        private void Awake()
        {
            if (movement == null)
            {
                movement = GetComponent<PatchyMovement>();
                Debug.LogWarning($"Movement reference was not set on {gameObject.name}. Auto-assigning.");
            }
            factory = new MovementStateFactory(movement, config);
        }

        private void Start()
        {
            // Start in the Idle state
            ChangeState(MovementState.Idle);
        }

        public void ChangeState(MovementState newState)
        {
            if (CurrentState == newState) return; // Avoid redundant state changes

            ExitState(CurrentState);
            CurrentState = newState;
            EnterState(newState);
        }

        private void EnterState(MovementState state)
        {
            currentState = factory.GetState(state);
            currentState.Enter();
            switch (state)
            {
                case MovementState.Idle:
                    animator?.SetInteger(AnimatorState, 0);
                    Debug.Log("Entering Idle State");
                    break;
                case MovementState.Walking:
                    animator?.SetInteger(AnimatorState, 1);
                    Debug.Log("Entering Walking State");
                    break;
                case MovementState.Jumping:
                    animator?.SetInteger(AnimatorState, 2);
                    Debug.Log("Entering Jumping State");
                    break;
                case MovementState.Dashing:
                    animator?.SetInteger(AnimatorState, 3);
                    Debug.Log("Entering Dashing State");
                    break;
                case MovementState.Sprinting:
                    animator?.SetInteger(AnimatorState, 4);
                    Debug.Log("Entering Sprinting State");
                    break;
            }
        }

        private void ExitState(MovementState state)
        {
            if (currentState != null)
            {
                currentState.Exit();
            }
            switch (state)
            {
                case MovementState.Idle:
                    Debug.Log("Exiting Idle State");
                    break;
                case MovementState.Walking:
                    Debug.Log("Exiting Walking State");
                    break;
                case MovementState.Jumping:
                    Debug.Log("Exiting Jumping State");
                    break;
                case MovementState.Dashing:
                    Debug.Log("Exiting Dashing State");
                    break;
                case MovementState.Sprinting:
                    Debug.Log("Exiting Sprinting State");
                    break;
            }
        }

        private void Update()
        {
            currentState?.Update();
        }

        private void FixedUpdate()
        {
            currentState?.FixedUpdate();
        }
    }
}
