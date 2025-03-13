using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyPatrol : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float minHeightDifference = 0.5f;
    
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    
    [Header("Wall Check")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallCheckDistance = 0.5f;

    [Header("Pathfinding Settings")]
    [SerializeField] private float pathUpdateRate = 0.5f;
    [SerializeField] private float nodeDistance = 0.5f;
    [SerializeField] private int maxPathNodes = 10;

    [Header("Platform Detection")]
    [SerializeField] private float platformCheckDistance = 2f;
    [SerializeField] private float verticalCheckDistance = 3f;
    [SerializeField] private float jumpCooldown = 0.5f;
    private float nextJumpTime;
    private bool canJumpToPlayer = false;
    
    private Rigidbody2D rb;
    private Transform player;
    private bool isGrounded;
    private bool isFacingRight = true;
    private Vector2 movement;
    private List<Vector2> pathNodes = new List<Vector2>();
    private float pathUpdateTimer;
    private Vector2 currentTarget;
    private bool isPathValid;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (player == null) return;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        pathUpdateTimer -= Time.deltaTime;
        if (pathUpdateTimer <= 0)
        {
            UpdatePathToPlayer();
            pathUpdateTimer = pathUpdateRate;
        }

        if (pathNodes.Count > 0)
        {
            currentTarget = pathNodes[0];
            Vector2 directionToTarget = (currentTarget - (Vector2)transform.position).normalized;
            float distanceToTarget = Vector2.Distance(transform.position, currentTarget);
            
            // Handle movement and jumping
            movement.x = directionToTarget.x;
            float heightDifference = currentTarget.y - transform.position.y;

            // Improved jumping logic
            if (isGrounded && Time.time >= nextJumpTime)
            {
                if (heightDifference > minHeightDifference)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                    nextJumpTime = Time.time + jumpCooldown;
                }
            }

            // Move to next node if close enough
            if (distanceToTarget < 0.1f)
            {
                pathNodes.RemoveAt(0);
            }

            // Handle sprite flipping
            if ((directionToTarget.x > 0 && !isFacingRight) || (directionToTarget.x < 0 && isFacingRight))
            {
                Flip();
            }
        }
    }

    private void FixedUpdate()
    {
        // Apply horizontal movement
        rb.linearVelocity = new Vector2(movement.x * moveSpeed, rb.linearVelocity.y);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    private void UpdatePathToPlayer()
    {
        pathNodes.Clear();
        Vector2 startPos = transform.position;
        Vector2 endPos = player.position;
        
        // Check vertical distance to player
        float heightDifference = endPos.y - startPos.y;
        
        // Check if there's a platform above or below that leads to the player
        CheckPlatformAccessibility(heightDifference);

        // If we can't reach the player directly, find alternative path
        if (!canJumpToPlayer)
        {
            FindAlternativePath(startPos, endPos);
        }
        else
        {
            // Direct path is possible
            pathNodes.Add(endPos);
        }
    }

    private void CheckPlatformAccessibility(float heightDifference)
    {
        Vector2 startPos = transform.position;
        Vector2 playerPos = player.position;
        canJumpToPlayer = false;

        // Ahora usamos platformCheckDistance para verificar plataformas lateralmente
        RaycastHit2D lateralCheck = Physics2D.Raycast(startPos, 
            (playerPos - startPos).normalized, 
            platformCheckDistance, 
            groundLayer);

        if (heightDifference > 0)
        {
            // Cast a ray upward to check for platforms
            RaycastHit2D upwardHit = Physics2D.Raycast(startPos, Vector2.up, verticalCheckDistance, groundLayer);
            if (upwardHit.collider != null)
            {
                // Check if we can reach this platform with our jump
                if (upwardHit.point.y - startPos.y <= jumpForce * 0.5f)
                {
                    // Check if we can reach the player from this platform
                    Vector2 platformEdgePos = new Vector2(startPos.x, upwardHit.point.y);
                    RaycastHit2D toPlayerHit = Physics2D.Linecast(platformEdgePos, playerPos, groundLayer);
                    if (!toPlayerHit)
                    {
                        canJumpToPlayer = true;
                    }
                }
            }
        }
        else if (heightDifference < 0)
        {
            // Cast a ray downward to check for lower platforms
            RaycastHit2D downwardHit = Physics2D.Raycast(startPos, Vector2.down, verticalCheckDistance, groundLayer);
            if (downwardHit.collider != null)
            {
                // Check if there's a clear path to the player from the lower platform
                Vector2 platformEdgePos = new Vector2(startPos.x, downwardHit.point.y);
                RaycastHit2D toPlayerHit = Physics2D.Linecast(platformEdgePos, playerPos, groundLayer);
                if (!toPlayerHit)
                {
                    canJumpToPlayer = true;
                }
            }
        }

        // Verificar si hay una pared bloqueando el camino
        RaycastHit2D wallHit = Physics2D.Raycast(wallCheck.position, 
            transform.right * (isFacingRight ? 1 : -1), 
            wallCheckDistance, 
            groundLayer);

        if (wallHit)
        {
            canJumpToPlayer = false;
        }
    }

    private void FindAlternativePath(Vector2 startPos, Vector2 endPos)
    {
        Vector2 currentPos = startPos;
        pathNodes.Add(currentPos);

        for (int i = 0; i < maxPathNodes; i++)
        {
            Vector2 directionToPlayer = (endPos - currentPos).normalized;
            float[] angles = { 0, 45, -45, 90, -90 };
            bool foundPath = false;

            foreach (float angle in angles)
            {
                Vector2 direction = Quaternion.Euler(0, 0, angle) * directionToPlayer;
                Vector2 nextPos = currentPos + direction * nodeDistance;
                
                // Verificar si hay plataforma accesible
                RaycastHit2D platformHit = Physics2D.Raycast(nextPos, Vector2.down, 1f, groundLayer);
                if (platformHit && !Physics2D.Linecast(currentPos, nextPos, groundLayer))
                {
                    nextPos.y = platformHit.point.y;
                    currentPos = nextPos;
                    pathNodes.Add(currentPos);
                    foundPath = true;
                    break;
                }
            }

            if (!foundPath || Vector2.Distance(currentPos, endPos) < nodeDistance)
            {
                if (pathNodes[pathNodes.Count - 1] != endPos)
                    pathNodes.Add(endPos);
                break;
            }
        }
    }
}
