using UnityEngine;
using Core.Interfaces;
using Core.Player.Movement;
using System;

namespace Core.Player.Movement.States
{
    public class DashingState : IMovementState
    {
        private readonly PatchyMovement movement;
        private readonly Rigidbody2D rb;
        private readonly MovementConfig config;
        private float dashTimeLeft;
        private MovementStateMachine stateMachine;

        public DashingState(PatchyMovement movement, Rigidbody2D rb, MovementConfig config, MovementStateMachine stateMachine)
        {
            this.movement = movement;
            this.rb = rb;
            this.config = config;
            this.stateMachine = stateMachine;
        }

        public void Enter()
        {
            dashTimeLeft = config.DashSettings.DashDuration;
            ApplyDashForce();
        }

        public void Update()
        {
            HandleInput(Vector2.zero);
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0)
            {
                RequestStateChange(MovementStateMachine.MovementState.Idle);
            }
        }

        public void FixedUpdate() { }

        public void Exit()
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.5f, rb.linearVelocity.y);
        }

        public void HandleInput(Vector2 input) { }

        public event Action<MovementStateMachine.MovementState> OnStateChangeRequested;

        public void RequestStateChange(MovementStateMachine.MovementState newState)
        {
            // Invoke the event here!
            OnStateChangeRequested?.Invoke(newState);
        }

        private void ApplyDashForce()
        {            
            Vector2 dashDirection = movement.transform.localScale.x > 0 ? Vector2.right : Vector2.left;
            rb.linearVelocity = dashDirection * config.DashSettings.DashSpeed;
        }
    }
}
