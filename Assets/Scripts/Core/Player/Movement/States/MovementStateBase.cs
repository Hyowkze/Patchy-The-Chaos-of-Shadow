using UnityEngine;
using Core.Interfaces;
using Player.InputSystem; // Actualizar este using
using System;
using Player.Movement;

namespace Core.Player.Movement.States
{
    public abstract class MovementStateBase : IMovementState
    {
        protected readonly PatchyMovement movement;
        protected readonly Rigidbody2D rb;
        protected readonly MovementConfig config;
        protected readonly MovementStateMachine stateMachine;

        public event Action<MovementStateMachine.MovementState> OnStateChangeRequested;

        protected MovementStateBase(PatchyMovement movement, Rigidbody2D rb, MovementConfig config, MovementStateMachine stateMachine)
        {
            this.movement = movement;
            this.rb = rb;
            this.config = config;
            this.stateMachine = stateMachine;
        }

        public virtual void Enter() { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void Exit() { }
        public virtual void HandleInput() { } // Add this line

        public void RequestStateChange(MovementStateMachine.MovementState newState)
        {
            OnStateChangeRequested?.Invoke(newState);
        }
    }
}
