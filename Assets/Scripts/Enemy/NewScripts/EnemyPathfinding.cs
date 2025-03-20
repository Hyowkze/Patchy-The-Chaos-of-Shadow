using UnityEngine;
using System.Collections.Generic;
using Core.Enemy;
using Core.Pathfinding;

namespace Core.Enemy
{
    [RequireComponent(typeof(EnemyDetection))]
    public class EnemyPathfinding : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private PathfindingConfig config;

        [Header("Pathfinding Settings")]
        [SerializeField] private float pathUpdateRate = 0.5f;
        [SerializeField] private bool debugPath;

        private EnemyDetection detection;
        private readonly List<Vector3> currentPath = new List<Vector3>();
        private float pathUpdateTimer;

        public IReadOnlyList<Vector3> CurrentPath => currentPath;

        private void Awake()
        {
            detection = GetComponent<EnemyDetection>();

            if (config == null)
            {
                Debug.LogError($"Missing PathfindingConfig on {gameObject.name}");
                enabled = false;
                return;
            }
        }

        private void Start()
        {
            UpdatePath();
        }

        private void Update()
        {
            if (!detection.PlayerFound) return;

            pathUpdateTimer -= Time.deltaTime;
            if (pathUpdateTimer <= 0f)
            {
                UpdatePath();
                pathUpdateTimer = pathUpdateRate;
            }
        }

        private void OnEnable()
        {
            if (detection != null)
            {
                // Suscribirse a eventos si es necesario
            }
        }

        private void OnDisable()
        {
            // Desuscribirse de eventos
        }

        public void UpdatePath()
        {
            if (!IsPathfindingPossible()) return;

            try
            {
                PathRequestManager.RequestPath(transform.position, detection.PlayerPosition, OnPathFound);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Pathfinding error on {gameObject.name}: {e.Message}");
            }
        }

        private void OnPathFound(List<Vector3> newPath)
        {
            currentPath.Clear();
            currentPath.AddRange(newPath);
        }

        private bool IsPathfindingPossible()
        {
            return detection != null &&
                   detection.PlayerFound;
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying || !debugPath) return;
            DrawPathGizmos();
        }

        private void DrawPathGizmos()
        {
            if (currentPath == null || currentPath.Count == 0) return;

            Gizmos.color = Color.blue;
            for (int i = 0; i < currentPath.Count - 1; i++)
            {
                Gizmos.DrawLine(currentPath[i], currentPath[i + 1]);
                Gizmos.DrawWireSphere(currentPath[i], config.nodeRadius);
            }
        }
    }
}
