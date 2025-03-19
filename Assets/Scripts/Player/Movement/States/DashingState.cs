using UnityEngine;
using Core.Interfaces;

namespace Core.Player.Movement.States
{
    public class DashingState : IMovementState
    {
        private readonly PatchyMovement movement;
        private readonly Rigidbody2D rb;
        private readonly MovementConfig config;
        private float dashTimeLeft;
        private readonly MovementStateMachine stateMachine; // Added

        public DashingState(PatchyMovement movement, Rigidbody2D rb, MovementConfig config, MovementStateMachine stateMachine) // Modified
        {
            this.movement = movement;
            this.rb = rb;
            this.config = config;
            this.stateMachine = stateMachine; // Added
        }

        public void Enter()
        {
            movement.GetComponent<Animator>()?.SetInteger("State", 3);
            dashTimeLeft = config.DashSettings.DashDuration;
            ApplyDashForce();
        }

        public void Update()
        {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0)
            {
                stateMachine.ChangeState(MovementStateMachine.MovementState.Idle); // Modified
            }
        }

        public void FixedUpdate() { }

        public void Exit()
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.5f, rb.linearVelocity.y);
        }

        public void HandleInput(Vector2 input) { }

        private void ApplyDashForce()
        {
            // Assuming you have a way to get the facing direction from PatchyMovement
            Vector2 dashDirection = movement.transform.localScale.x > 0 ? Vector2.right : Vector2.left;
            rb.linearVelocity = dashDirection * config.DashSettings.DashSpeed;
        }
    }
}
