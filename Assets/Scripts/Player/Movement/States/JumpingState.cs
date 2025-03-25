using UnityEngine;
using Core.Player.Movement.States;
using Player.Movement;
using Player.InputSystem;

namespace Core.Player.Movement.States
{
    public class JumpingState : MovementStateBase
    {
        public JumpingState(PatchyMovement movement, Rigidbody2D rb, MovementConfig config, MovementStateMachine stateMachine)
            : base(movement, rb, config, stateMachine) { }

        public override void Enter()
        {
            rb.AddForce(Vector2.up * config.JumpForce, ForceMode2D.Impulse);
        }

        public override void HandleInput() // Change from Update to HandleInput
        {
            if (movement.IsGrounded)
            {
                if (PlayerInputHandler.Instance.MoveInput.x == 0)
                {
                    RequestStateChange(MovementStateMachine.MovementState.Idle);
                }
                else
                {
                    RequestStateChange(MovementStateMachine.MovementState.Walking);
                }
            }
        }

        public override void FixedUpdate()
        {
            float targetVelocityX = PlayerInputHandler.Instance.MoveInput.x * config.MoveSpeed;
            float newVelocityX = Mathf.Lerp(rb.linearVelocity.x, targetVelocityX, config.AirControl * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(newVelocityX, rb.linearVelocity.y);
        }
    }
}
