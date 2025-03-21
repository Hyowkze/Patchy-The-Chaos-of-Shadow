using UnityEngine;
using Core.Player.Movement.States;
using Player.Movement;

namespace Core.Player.Movement.States
{
    public class DashingState : MovementStateBase
    {
        private float dashTimeLeft;

        public DashingState(PatchyMovement movement, Rigidbody2D rb, MovementConfig config, MovementStateMachine stateMachine)
            : base(movement, rb, config, stateMachine) { }

        public override void Enter()
        {
            dashTimeLeft = config.DashSettings.DashDuration;
            ApplyDashForce();
        }

        public override void HandleInput() // Change from Update to HandleInput
        {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0)
            {
                RequestStateChange(MovementStateMachine.MovementState.Idle);
            }
        }

        public override void Exit()
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.5f, rb.linearVelocity.y);
        }

        private void ApplyDashForce()
        {
            Vector2 dashDirection = movement.transform.localScale.x > 0 ? Vector2.right : Vector2.left;
            rb.linearVelocity = dashDirection * config.DashSettings.DashSpeed;
        }
    }
}
