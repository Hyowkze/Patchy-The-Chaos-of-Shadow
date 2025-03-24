using UnityEngine;
using Core.Utils;
using Core.Interfaces;
using Core.Combat;
using Core.Player.Movement;

namespace Player.Movement
{
    public class MovementStateMachine : ComponentRequester
    {
        public enum MovementState
        {
            Idle,
            Walking,
            Jumping,
            Dashing,
            Sprinting,
            Attacking,
            SpecialAttacking
        }

        public MovementState CurrentState { get; private set; }

        private MovementStateFactory stateFactory;
        private CombatSystem combatSystem;
        private IMovementState currentState;
        private PatchyMovement patchyMovement;

        protected override void Awake()
        {
            base.Awake();
            combatSystem = RequestComponent<CombatSystem>();
            patchyMovement = RequestComponent<PatchyMovement>();
            // Create the state factory here, after components are requested
            stateFactory = new MovementStateFactory(patchyMovement, patchyMovement.moveConfig, this, combatSystem);
        }

        protected override void Start()
        {
            ChangeState(MovementState.Idle);
        }

        public void ChangeState(MovementState newState)
        {
            if (CurrentState == newState) return;

            ExitState(CurrentState);
            CurrentState = newState;
            EnterState(newState);
        }

        private void EnterState(MovementState state)
        {
            currentState = stateFactory.CreateState(state);
            currentState.OnStateChangeRequested += ChangeState;
            currentState.Enter();
        }

        private void ExitState(MovementState state)
        {
            if (currentState != null)
            {
                currentState.Exit();
                currentState.OnStateChangeRequested -= ChangeState;
            }
        }

        public void UpdateState()
        {
            currentState.Update();
        }

        public void FixedUpdateState()
        {
            currentState.FixedUpdate();
        }

        public void HandleInput()
        {
            if (currentState != null)
            {
                currentState.HandleInput();
            }
        }
    }
}
