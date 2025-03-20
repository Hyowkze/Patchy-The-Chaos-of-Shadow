using UnityEngine;
using Core.Interfaces;
using Player.Input;
using Core.Player.Movement;
using System;

namespace Core.Player.Movement.States
{
    public class WalkingState : IMovementState
    {
        private readonly PatchyMovement movement;
        private readonly Rigidbody2D rb;
        private readonly MovementConfig config;
        private PlayerInputHandler inputHandler;
        private MovementStateMachine stateMachine;

        public WalkingState(PatchyMovement movement, Rigidbody2D rb, MovementConfig config, MovementStateMachine stateMachine)
        {
            this.movement = movement;
            this.rb = rb;
            this.config = config;
            inputHandler = PlayerInputHandler.Instance;
            this.stateMachine = stateMachine;
        }

        public void Enter()
        {
        }

        public void Update()
        {
            HandleInput(inputHandler.MoveInput);
        }

        public void FixedUpdate()
        {
            Vector2 currentVelocity = rb.linearVelocity;
            float targetVelocity = inputHandler.MoveInput.x * config.MoveSpeed;

            rb.linearVelocity = new Vector2(
                Mathf.Lerp(currentVelocity.x, targetVelocity, config.GroundFriction * Time.fixedDeltaTime),
                currentVelocity.y
            );
        }

        public void Exit() { }

        public void HandleInput(Vector2 input)
        {
            if (inputHandler.MoveInput.x == 0)
            {
                RequestStateChange(MovementStateMachine.MovementState.Idle);
            }
        }

        public event Action<MovementStateMachine.MovementState> OnStateChangeRequested;

        public void RequestStateChange(MovementStateMachine.MovementState newState)
        {
            // Invoke the event here!
            OnStateChangeRequested?.Invoke(newState);
        }
    }
}
