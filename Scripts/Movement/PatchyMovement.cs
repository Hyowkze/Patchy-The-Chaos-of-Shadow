using UnityEngine;

public class PatchyMovement : MonoBehaviour
{
    [SerializeField] private MovementStateMachine stateMachine;
    [SerializeField] private DashBehavior dashBehavior;
    [SerializeField] private SprintBehavior sprintBehavior;
    
    // ...existing code...

    private void HandleMovementState()
    {
        if (dashBehavior.IsDashing)
        {
            stateMachine.ChangeState(MovementStateMachine.MovementState.Dashing);
        }
        else if (sprintBehavior.IsSprinting)
        {
            stateMachine.ChangeState(MovementStateMachine.MovementState.Sprinting);
        }
        else if (!IsGrounded)
        {
            stateMachine.ChangeState(MovementStateMachine.MovementState.Jumping);
        }
        else if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            stateMachine.ChangeState(MovementStateMachine.MovementState.Walking);
        }
        else
        {
            stateMachine.ChangeState(MovementStateMachine.MovementState.Idle);
        }
    }

    private void HandleCollisions(Collision2D collision)
    {
        if (IsGroundCollision(collision))
        {
            HandleGroundCollision();
        }
        else if (IsWallCollision(collision))
        {
            HandleWallCollision();
        }
    }

    private bool IsGroundCollision(Collision2D collision)
    {
        return collision.contacts[0].normal.y > 0.7f;
    }

    private bool IsWallCollision(Collision2D collision)
    {
        return Mathf.Abs(collision.contacts[0].normal.x) > 0.7f;
    }

    // ...existing code...
}
