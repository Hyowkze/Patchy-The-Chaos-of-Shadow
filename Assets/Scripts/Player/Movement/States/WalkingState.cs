//e:\Curso\Proyecto\Assets\Scripts\Player\Movement\States\WalkingState.cs
using UnityEngine;
using Core.Player.Movement.States;
using Player.Movement;
using Player.InputSystem; // Actualizar este using

namespace Core.Player.Movement.States
{
    public class WalkingState : MovementStateBase
    {
        public WalkingState(PatchyMovement movement, Rigidbody2D rb, MovementConfig config, MovementStateMachine stateMachine)
            : base(movement, rb, config, stateMachine) { }

        public override void HandleInput() // Change from Update to HandleInput
        {
            if (PlayerInputHandler.Instance.MoveInput.x == 0)
            {
                RequestStateChange(MovementStateMachine.MovementState.Idle);
            }
        }

        public override void FixedUpdate()
        {
            Vector2 currentVelocity = rb.linearVelocity;
            float targetVelocity = PlayerInputHandler.Instance.MoveInput.x * config.MoveSpeed;

            rb.linearVelocity = new Vector2(
                Mathf.Lerp(currentVelocity.x, targetVelocity, config.GroundFriction * Time.fixedDeltaTime),
                currentVelocity.y
            );
        }
        public override void Update()
        {
        }
    }
}
