using System;
using Player.Movement;

namespace Core.Interfaces
{
    public interface IMovementState
    {
        event Action<MovementStateMachine.MovementState> OnStateChangeRequested;
        void Enter();
        void Update();
        void FixedUpdate();
        void Exit();
        void HandleInput(); // Add this line
    }
}
