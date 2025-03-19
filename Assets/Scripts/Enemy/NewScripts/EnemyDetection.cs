using UnityEngine;
using System.Collections.Generic;
using Core.Enemy;

namespace Core.Enemy 
{
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

        private Transform playerTransform;
        private Vector3 lastKnownPlayerPosition;
        private readonly RaycastHit2D[] rayResults = new RaycastHit2D[1];
        
        // Public properties para acceso externo
        public bool IsGrounded { get; private set; }
        public bool PlayerMovingUp { get; private set; }
        public bool ShouldDrop { get; private set; }
        public bool InJumpZone { get; private set; }
        public bool PlayerFound { get; private set; }
        public Vector3 PlayerPosition => playerTransform.position;
        public List<Vector2> PlatformTransitions { get; } = new List<Vector2>();

        private void Awake()
        {
            ValidateReferences();
            CacheReferences();
        }

        private void ValidateReferences()
        {
            if (groundCheck == null || wallCheck == null)
            {
                Debug.LogError($"Missing check points on {gameObject.name}");
                enabled = false;
            }
        }

        private void CacheReferences()
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (playerTransform == null)
            {
                Debug.LogError("Player not found in scene!");
                enabled = false;
            }
        }

        private void Start()
        {
            lastKnownPlayerPosition = playerTransform.position;
            UpdatePlayerDetection();
        }

        private void Update()
        {
            if (!enabled) return;
            
            UpdateDetections();
        }

        private void UpdateDetections()
        {
            UpdatePlayerDetection();
            TrackPlayerMovement();
            UpdateGroundDetection();
            UpdateZoneDetections();
            UpdateTransitionPoints();
        }

        private void TrackPlayerMovement()
        {
            if (playerTransform == null) return;
            
            float verticalMovement = playerTransform.position.y - lastKnownPlayerPosition.y;
            PlayerMovingUp = verticalMovement > 0.05f;
            ShouldDrop = verticalMovement < -0.05f;
            lastKnownPlayerPosition = playerTransform.position;
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
            if (playerTransform == null) return false;
            
            float heightDifference = playerTransform.position.y - transform.position.y;
            return Mathf.Abs(heightDifference) > minHeightDifference && 
                   !Physics2D.Linecast(transform.position, playerTransform.position, groundLayer);
        }

        private void UpdatePlayerDetection()
        {
            if (playerTransform == null)
            {
                TryFindPlayer();
                return;
            }

            PlayerFound = Vector2.Distance(transform.position, playerTransform.position) <= 20f;
        }

        private void TryFindPlayer()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
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
}