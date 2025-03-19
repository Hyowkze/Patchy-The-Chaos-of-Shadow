using UnityEngine;

namespace Core.Interfaces
{
    public interface IMovementState
    {
        void Enter();
        void Update();
        void FixedUpdate();
        void Exit();
        void HandleInput(Vector2 input);
    }
}
