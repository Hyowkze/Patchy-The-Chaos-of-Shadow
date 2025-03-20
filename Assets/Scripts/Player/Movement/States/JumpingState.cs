using UnityEngine;
using Core.Interfaces;
using Player.Input;
using Core.Player.Movement;
using System;

namespace Core.Player.Movement.States
{
    public class JumpingState : IMovementState
    {
        private readonly PatchyMovement movement;
        private readonly Rigidbody2D rb;
        private readonly MovementConfig config;
        private PlayerInputHandler inputHandler;
        private MovementStateMachine stateMachine;

        public JumpingState(PatchyMovement movement, Rigidbody2D rb, MovementConfig config, MovementStateMachine stateMachine)
        {
            this.movement = movement;
            this.rb = rb;
            this.config = config;
            inputHandler = PlayerInputHandler.Instance;
            this.stateMachine = stateMachine;
        }

        public void Enter()
        {
            rb.AddForce(Vector2.up * config.JumpForce, ForceMode2D.Impulse);
        }

        public void Update()
        {
            HandleInput(inputHandler.MoveInput);
        }

        public void FixedUpdate()
        {
            float targetVelocityX = inputHandler.MoveInput.x * config.MoveSpeed;
            float newVelocityX = Mathf.Lerp(rb.linearVelocity.x, targetVelocityX, config.AirControl * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(newVelocityX, rb.linearVelocity.y);
        }

        public void Exit() { }

        public void HandleInput(Vector2 input)
        {
            if (movement.IsGrounded)
            {
                if (inputHandler.MoveInput.x == 0)
                {
                    RequestStateChange(MovementStateMachine.MovementState.Idle);
                }
                else
                {
                    RequestStateChange(MovementStateMachine.MovementState.Walking);
                }
            }
        }

        public event Action<MovementStateMachine.MovementState> OnStateChangeRequested;

        public void RequestStateChange(MovementStateMachine.MovementState newState)
        {
            // Invoke the event here!
            OnStateChangeRequested?.Invoke(newState);
            //stateMachine.ChangeState(newState); // Remove this line
        }
    }
}
