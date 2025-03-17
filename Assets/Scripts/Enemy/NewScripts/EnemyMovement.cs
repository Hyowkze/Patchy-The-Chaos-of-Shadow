using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyDetection))]
[RequireComponent(typeof(EnemyPathfinding))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float nodeDistance = 0.5f;
    [SerializeField] private float jumpForwardForce = 2.2f;
    [SerializeField] private float verticalJumpForce = 12f;
    [SerializeField] private float jumpCooldown = 0.4f;

    [Header("Physics Settings")]
    [SerializeField] private float groundDrag = 5f;
    [SerializeField] private float airDrag = 1f;

    private Rigidbody2D rb;
    private EnemyDetection detection;
    private EnemyPathfinding pathfinding;
    private int targetPathIndex;
    private float nextJumpTime;
    private bool isFacingRight = true;
    private Vector2 movementDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        detection = GetComponent<EnemyDetection>();
        pathfinding = GetComponent<EnemyPathfinding>();
    }

    private void Update()
    {
        UpdateMovementDirection();
        HandleJumping();
        AdjustPhysicsMaterial();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void UpdateMovementDirection()
    {
        movementDirection = Vector2.zero;
        
        if (pathfinding.CurrentPath == null || pathfinding.CurrentPath.Count == 0) return;

        Vector2 currentWaypoint = pathfinding.CurrentPath[targetPathIndex];
        movementDirection = (currentWaypoint - (Vector2)transform.position).normalized;

        if (Vector2.Distance(transform.position, currentWaypoint) < nodeDistance)
        {
            targetPathIndex++;
            if (targetPathIndex >= pathfinding.CurrentPath.Count)
                pathfinding.UpdatePath();
        }

        HandleFlipping(movementDirection);
    }

    private void ApplyMovement()
    {
        if (detection.IsGrounded)
        {
            rb.linearVelocity = new Vector2(movementDirection.x * moveSpeed, rb.linearVelocity.y);
        }
    }

    private void HandleJumping()
    {
        if (CanJump())
        {
            ExecuteJump();
            nextJumpTime = Time.time + jumpCooldown;
        }
    }

    private bool CanJump()
    {
        return detection.IsGrounded && 
               Time.time >= nextJumpTime && 
               (detection.CheckValidJump() || detection.CheckWallForward() || detection.InJumpZone);
    }

    private void ExecuteJump()
    {
        Vector2 jumpDirection = (detection.PlayerPosition - transform.position).normalized;
        float horizontalForce = Mathf.Clamp(Mathf.Abs(jumpDirection.x), 0.5f, 3f);
        
        rb.AddForce(new Vector2(
            jumpDirection.x * horizontalForce * jumpForwardForce,
            verticalJumpForce
        ), ForceMode2D.Impulse);
    }

    private void AdjustPhysicsMaterial()
    {
        rb.linearDamping = detection.IsGrounded ? groundDrag : airDrag;
    }

    private void HandleFlipping(Vector2 direction)
    {
        if ((direction.x > 0 && !isFacingRight) || (direction.x < 0 && isFacingRight))
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    public void ForceJump(Vector2 direction, float forceMultiplier = 1f)
    {
        rb.AddForce(new Vector2(
            direction.x * jumpForwardForce * forceMultiplier,
            verticalJumpForce * forceMultiplier
        ), ForceMode2D.Impulse);
    }
}