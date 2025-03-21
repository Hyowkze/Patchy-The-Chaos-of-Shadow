using UnityEngine;
using System.Collections.Generic;
using Core.Enemy;
using Core.Enemy.AI;
using Core.Utils;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyDetection))]
[RequireComponent(typeof(EnemyPathfinding))]
[RequireComponent(typeof(EnemyAIStateMachine))]
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

    [Header("Patrol Settings")]
    [SerializeField] private List<Transform> patrolPoints;
    [SerializeField] private float patrolWaitTime = 2f;
    [SerializeField] private float patrolWaitTimeVariance = 0.5f;
    [SerializeField] private bool randomPatrol = false;
    private int currentPatrolIndex = 0;
    private float patrolTimer = 0f;
    private bool isWaiting = false;

    private Rigidbody2D rb;
    private EnemyDetection detection;
    private EnemyPathfinding pathfinding;
    private EnemyAIStateMachine stateMachine;
    private int targetPathIndex;
    private float nextJumpTime;
    private bool isFacingRight = true;
    private Vector2 movementDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        detection = GetComponent<EnemyDetection>();
        pathfinding = GetComponent<EnemyPathfinding>();
        stateMachine = GetComponent<EnemyAIStateMachine>();
    }

    private void Update()
    {
        UpdateMovementDirection();
        HandleJumping();
        AdjustPhysicsMaterial();

        if (stateMachine.CurrentState == EnemyAIStateMachine.AIState.Patrolling)
        {
            Patrol();
        }
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void UpdateMovementDirection()
    {
        movementDirection = Vector2.zero;

        if (stateMachine.CurrentState == EnemyAIStateMachine.AIState.Chasing)
        {
            if (pathfinding.CurrentPath == null || pathfinding.CurrentPath.Count == 0) return;

            Vector2 currentWaypoint = pathfinding.CurrentPath[targetPathIndex];
            movementDirection = (currentWaypoint - (Vector2)transform.position).normalized;

            if (Vector2.Distance(transform.position, currentWaypoint) < nodeDistance)
            {
                targetPathIndex++;
                if (targetPathIndex >= pathfinding.CurrentPath.Count)
                    pathfinding.UpdatePath();
            }
        }
        else if (stateMachine.CurrentState == EnemyAIStateMachine.AIState.Patrolling)
        {
            if (patrolPoints.Count == 0) return;

            Vector2 currentWaypoint = patrolPoints[currentPatrolIndex].position;

            if (pathfinding.CurrentPath != null && pathfinding.CurrentPath.Count > 0)
            {
                currentWaypoint = pathfinding.CurrentPath[targetPathIndex];
                movementDirection = (currentWaypoint - (Vector2)transform.position).normalized;

                if (Vector2.Distance(transform.position, currentWaypoint) < nodeDistance)
                {
                    targetPathIndex++;
                    if (targetPathIndex >= pathfinding.CurrentPath.Count)
                    {
                        targetPathIndex = 0;
                        isWaiting = true;
                        patrolTimer = patrolWaitTime + Random.Range(-patrolWaitTimeVariance, patrolWaitTimeVariance);
                    }
                }
            }
            else
            {
                movementDirection = (currentWaypoint - (Vector2)transform.position).normalized;

                if (Vector2.Distance(transform.position, currentWaypoint) < nodeDistance)
                {
                    if (!isWaiting)
                    {
                        isWaiting = true;
                        patrolTimer = patrolWaitTime + Random.Range(-patrolWaitTimeVariance, patrolWaitTimeVariance);
                    }
                }
            }
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
            SpriteUtils.FlipSprite(transform, ref isFacingRight);
        }
    }

    private void Patrol()
    {
        if (patrolPoints.Count == 0) return;

        if (isWaiting)
        {
            patrolTimer -= Time.deltaTime;
            if (patrolTimer <= 0)
            {
                isWaiting = false;
                if (randomPatrol)
                {
                    currentPatrolIndex = Random.Range(0, patrolPoints.Count);
                }
                else
                {
                    currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
                }
                if (pathfinding != null)
                {
                    pathfinding.UpdatePath();
                    targetPathIndex = 0; // Reset the targetPathIndex
                }
            }
        }
        else
        {
            if (pathfinding != null)
            {
                pathfinding.UpdatePath();
                targetPathIndex = 0; // Reset the targetPathIndex
            }
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
