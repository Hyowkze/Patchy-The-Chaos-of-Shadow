using UnityEngine;
using Core.Player.Movement; 

namespace Core.Interfaces
{
    public interface IMovementState
    {
        /// <summary>
        /// Called when the state is entered.
        /// </summary>
        void Enter();

        /// <summary>
        /// Called every frame.
        /// </summary>
        void Update();

        /// <summary>
        /// Called every physics update.
        /// </summary>
        void FixedUpdate();

        /// <summary>
        /// Called when the state is exited.
        /// </summary>
        void Exit();

        /// <summary>
        /// Handles player input within the context of this state.
        /// </summary>
        /// <param name="input">The current player input.</param>
        void HandleInput(Vector2 input);

        /// <summary>
        /// Event triggered when this state wants to request a change to another state.
        /// </summary>
        event System.Action<MovementStateMachine.MovementState> OnStateChangeRequested;

        /// <summary>
        /// Requests a state change to the specified new state.
        /// </summary>
        /// <param name="newState">The new state to transition to.</param>
        void RequestStateChange(MovementStateMachine.MovementState newState);
    }
}
