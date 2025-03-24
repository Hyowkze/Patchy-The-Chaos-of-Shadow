using UnityEngine;
using Core.Player.Movement.States;
using Player.Movement;
using Player.Input;

namespace Core.Player.Movement.States
{
    public class IdleState : MovementStateBase
    {
        public IdleState(PatchyMovement movement, Rigidbody2D rb, MovementConfig config, MovementStateMachine stateMachine)
            : base(movement, rb, config, stateMachine) { }

        public override void HandleInput() // Change from Update to HandleInput
        {
            if (PlayerInputHandler.Instance.MoveInput.x != 0)
            {
                RequestStateChange(MovementStateMachine.MovementState.Walking);
            }
        }

        public override void FixedUpdate()
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
    }
}