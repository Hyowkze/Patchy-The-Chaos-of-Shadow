using UnityEngine;
using Core.Interfaces;
using Player.Input;

namespace Core.Player.Movement.States
{
    public class SprintingState : IMovementState
    {
        private readonly PatchyMovement movement;
        private readonly Rigidbody2D rb;
        private readonly MovementConfig config;
        private PlayerInputHandler inputHandler;
        private readonly MovementStateMachine stateMachine; // Added

        public SprintingState(PatchyMovement movement, Rigidbody2D rb, MovementConfig config)
        {
            this.movement = movement;
            this.rb = rb;
            this.config = config;
            inputHandler = PlayerInputHandler.Instance;
            this.stateMachine = movement.GetComponent<MovementStateMachine>(); // Added
        }

        public void Enter()
        {
            movement.GetComponent<Animator>()?.SetInteger("State", 4);
        }

        public void Update() { }

        public void FixedUpdate()
        {
            Vector2 currentVelocity = rb.linearVelocity;
            float targetVelocity = inputHandler.MoveInput.x * config.MoveSpeed * config.SprintSettings.SprintMultiplier;

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
                stateMachine.ChangeState(MovementStateMachine.MovementState.Idle); 
            }
            else if (inputHandler.SprintValue == 0)
            {
                stateMachine.ChangeState(MovementStateMachine.MovementState.Walking); 
            }
        }
    }
}
