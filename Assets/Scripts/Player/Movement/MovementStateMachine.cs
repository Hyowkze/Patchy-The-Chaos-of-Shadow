using UnityEngine;
using Core.Player.Movement;
using Core.Interfaces;
using Player.Movement; 

namespace Core.Player.Movement
{
    [RequireComponent(typeof(PatchyMovement))]
    public class MovementStateMachine : MonoBehaviour
    {
        [SerializeField] private PatchyMovement movement;
        [SerializeField] private Animator animator;
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

<<<<<<< Updated upstream
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
            factory = new MovementStateFactory(movement, config, this); // Line 42
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
            currentState = factory.GetState(state); // Line 26
            currentState.OnStateChangeRequested += ChangeState; 
            currentState.Enter();
        UpdateAnimator(state);
        }

        private void ExitState(MovementState state)
        {
            if (currentState != null)
            {
                currentState.Exit();
                currentState.OnStateChangeRequested -= ChangeState;
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

        private void UpdateAnimator(MovementState state)
        {
<<<<<<< Updated upstream
            switch (state)
            {
                case MovementState.Idle:
                    animator?.SetInteger(AnimatorState, 0);
                    break;
                case MovementState.Walking:
                    animator?.SetInteger(AnimatorState, 1);
                    break;
                case MovementState.Jumping:
                    animator?.SetInteger(AnimatorState, 2);
                    break;
                case MovementState.Dashing:
                    animator?.SetInteger(AnimatorState, 3);
                    break;
                case MovementState.Sprinting:
                    animator?.SetInteger(AnimatorState, 4);
                    break;
            }
        }
    }
}
