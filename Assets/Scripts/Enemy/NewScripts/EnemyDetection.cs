using UnityEngine;
using System.Collections.Generic;

public class EnemyDetection : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask jumpZoneLayer;
    [SerializeField] private LayerMask dropZoneLayer;
    [SerializeField] private float jumpZoneRadius = 3f;
    [SerializeField] private float dropCheckDistance = 3f;
    [SerializeField] private float transitionDetectionRadius = 0.5f;
    [SerializeField] private float groundCheckRadius = 0.4f;
    [SerializeField] private float wallCheckDistance = 1.2f;
    [SerializeField] private float minHeightDifference = 2.5f;

    [Header("References")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;

    private Transform player;
    private Vector3 playerPreviousPosition;
    
    // Public properties para acceso externo
    public bool IsGrounded { get; private set; }
    public bool PlayerMovingUp { get; private set; }
    public bool ShouldDrop { get; private set; }
    public bool InJumpZone { get; private set; }
    public bool PlayerFound { get; private set; }
    public Vector3 PlayerPosition => player.position;
    public List<Vector2> PlatformTransitions { get; } = new List<Vector2>();

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player == null) Debug.LogError("Player no encontrado!");
        playerPreviousPosition = player.position;
        UpdatePlayerDetection();
    }

    private void Update()
    {
        UpdatePlayerDetection();
        TrackPlayerMovement();
        UpdateGroundDetection();
        UpdateZoneDetections();
        UpdateTransitionPoints();
    }

    private void TrackPlayerMovement()
    {
        if (player == null) return;
        
        float verticalMovement = player.position.y - playerPreviousPosition.y;
        PlayerMovingUp = verticalMovement > 0.05f;
        ShouldDrop = verticalMovement < -0.05f;
        playerPreviousPosition = player.position;
    }

    private void UpdateGroundDetection()
    {
        IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void UpdateZoneDetections()
    {
        // Detección de Jump Zones
        InJumpZone = Physics2D.OverlapCircle(transform.position, jumpZoneRadius, jumpZoneLayer);

        // Detección de Drop Zones
        ShouldDrop = ShouldDrop || Physics2D.OverlapCircle(transform.position, dropCheckDistance, dropZoneLayer);
    }

    private void UpdateTransitionPoints()
    {
        PlatformTransitions.Clear();
        Collider2D[] nearbyPoints = Physics2D.OverlapCircleAll(
            transform.position, 
            transitionDetectionRadius,
            LayerMask.GetMask("JumpPoints", "DropPoints"));
        
        foreach (Collider2D point in nearbyPoints)
        {
            PlatformTransitions.Add(point.transform.position);
        }
    }

    public bool CheckWallForward()
    {
        return Physics2D.Raycast(
            wallCheck.position, 
            transform.right, 
            wallCheckDistance, 
            groundLayer
        );
    }

    public bool CheckValidJump()
    {
        if (player == null) return false;
        
        float heightDifference = player.position.y - transform.position.y;
        return Mathf.Abs(heightDifference) > minHeightDifference && 
               !Physics2D.Linecast(transform.position, player.position, groundLayer);
    }

    private void UpdatePlayerDetection()
    {
        PlayerFound = player != null && Vector2.Distance(transform.position, player.position) <= 20f;
    }

    private void OnDrawGizmosSelected()
    {
        // Dibuja área de detección de suelo
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        // Dibuja área de detección de saltos
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, jumpZoneRadius);

        // Dibuja área de detección de caídas
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dropCheckDistance);

        // Dibuja línea de chequeo de pared
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(wallCheck.position, transform.right * wallCheckDistance);
    }
}