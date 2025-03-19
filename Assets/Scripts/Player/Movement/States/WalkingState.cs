using UnityEngine;
using Core.Interfaces;
using Player.Input; 
using Core.Player.Movement; 

namespace Core.Player.Movement.States
{
    public class WalkingState : IMovementState
    {
        private readonly PatchyMovement movement;
        private readonly Rigidbody2D rb;
        private readonly MovementConfig config;
        private PlayerInputHandler inputHandler; 
        private readonly MovementStateMachine stateMachine; 

        public WalkingState(PatchyMovement movement, MovementConfig config) 
        {
            this.movement = movement;
            this.rb = movement.GetComponent<Rigidbody2D>(); 
            this.config = config;
            inputHandler = PlayerInputHandler.Instance; 
            this.stateMachine = movement.GetComponent<MovementStateMachine>(); 
        }

        public void Enter()
        {
            movement.GetComponent<Animator>()?.SetInteger("State", 1);
        }

        public void Update() { }

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
            //if (input.x == 0) // Removed
            if (inputHandler.MoveInput.x == 0) // Modified: Use inputHandler.MoveInput.x
            {
                stateMachine.ChangeState(MovementStateMachine.MovementState.Idle); // Added
            }
        }
    }
}
