using UnityEngine;
using Core.Interfaces;
using Player.Input;

namespace Core.Player.Movement.States
{
    public class JumpingState : IMovementState
    {
        private readonly PatchyMovement movement;
        private readonly Rigidbody2D rb; // Added
        private readonly MovementConfig config; // Added
        private PlayerInputHandler inputHandler; // Added

        public JumpingState(PatchyMovement movement, Rigidbody2D rb, MovementConfig config) // Modified
        {
            this.movement = movement;
            this.rb = rb; // Added
            this.config = config; // Added
            inputHandler = PlayerInputHandler.Instance; // Added
        }

        public void Enter()
        {
            movement.GetComponent<Animator>()?.SetInteger("State", 2);
        }

        public void Update() { }

        public void FixedUpdate()
        {
            Vector2 currentVelocity = rb.linearVelocity;
            float targetVelocity = inputHandler.MoveInput.x * config.MoveSpeed;

            rb.linearVelocity = new Vector2(
                Mathf.Lerp(currentVelocity.x, targetVelocity, config.AirControl * Time.fixedDeltaTime),
                currentVelocity.y
            );
        }

        public void Exit() { }

        public void HandleInput(Vector2 input)
        {
            if (inputHandler.MoveInput.x == 0)
            {
                //movement.ChangeState(MovementStateMachine.MovementState.Idle);
            }
        }
    }
}
