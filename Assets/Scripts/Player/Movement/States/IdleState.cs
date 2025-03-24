using UnityEngine;
using Core.Interfaces;
using Player.Input;
using Core.Player.Movement;
using System;

namespace Core.Player.Movement.States
{
    public class IdleState : IMovementState
    {
        private readonly PatchyMovement movement;
        private readonly Rigidbody2D rb;
        private readonly MovementConfig config;
        private PlayerInputHandler inputHandler;
        private MovementStateMachine stateMachine;

        public IdleState(PatchyMovement movement, Rigidbody2D rb, MovementConfig config, MovementStateMachine stateMachine)
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
            if (rb != null) // Add null check here
            {
                Vector2 currentVelocity = rb.linearVelocity;
                rb.linearVelocity = new Vector2(
                    Mathf.Lerp(currentVelocity.x, 0, config.GroundFriction * Time.fixedDeltaTime),
                    currentVelocity.y
                );
            }
            else
            {
                Debug.LogError("Rigidbody2D is null in IdleState.FixedUpdate()!");
            }
        }

        public void Exit() { }

        public void HandleInput(Vector2 input)
        {
            if (inputHandler.MoveInput.x != 0)
            {
                RequestStateChange(MovementStateMachine.MovementState.Walking);
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