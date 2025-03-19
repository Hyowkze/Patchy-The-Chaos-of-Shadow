using UnityEngine;
using Core.Interfaces;
using Player.Input;
using Core.Player.Movement; // Added this using directive

namespace Core.Player.Movement.States
{
    public class IdleState : IMovementState
    {
        private readonly PatchyMovement movement;
        private readonly Rigidbody2D rb;
        private readonly MovementConfig config;
        private PlayerInputHandler inputHandler;
        private readonly MovementStateMachine stateMachine; // Added

        public IdleState(PatchyMovement movement, Rigidbody2D rb, MovementConfig config)
        {
            this.movement = movement;
            this.rb = rb;
            this.config = config;
            inputHandler = PlayerInputHandler.Instance;
            this.stateMachine = movement.GetComponent<MovementStateMachine>(); // Added
        }

        public void Enter()
        {
            movement.GetComponent<Animator>()?.SetInteger("State", 0);
        }

        public void Update() { }

        public void FixedUpdate()
        {
            Vector2 currentVelocity = rb.linearVelocity;
            rb.linearVelocity = new Vector2(
                Mathf.Lerp(currentVelocity.x, 0, config.GroundFriction * Time.fixedDeltaTime),
                currentVelocity.y
            );
        }

        public void Exit() { }

        public void HandleInput(Vector2 input)
        {
            if (inputHandler.MoveInput.x != 0)
            {
                //movement.ChangeState(MovementStateMachine.MovementState.Walking); // Removed
                stateMachine.ChangeState(MovementStateMachine.MovementState.Walking); // Added
            }
        }
    }
}
