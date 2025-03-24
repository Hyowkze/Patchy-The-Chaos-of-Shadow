using UnityEngine;
using Core.Pathfinding;
using System.Collections.Generic;
using Core.Utils;

namespace Core.Enemy
{
    public class EnemyPathfinding : MonoBehaviour
    {
        [SerializeField] private PathfindingConfig config;
        [SerializeField] private Transform target;
        [SerializeField] private float updatePathInterval = 0.5f;
        [SerializeField] private float stoppingDistance = 0.5f;
        [SerializeField] private bool showGizmos = true;

        private float nodeRadius;
        private float timer;
        private List<Vector3> path;
        public List<Vector3> CurrentPath => path;
        private int targetIndex;
        private EnemyMovement enemyMovement;

        private void Awake()
        {
            enemyMovement = GetComponent<EnemyMovement>();
        }

        private void Start()
        {
            if (config == null)
            {
                Debug.LogError("PathfindingConfig is not assigned in EnemyPathfinding!");
                enabled = false;
                return;
            }
            if (target == null)
            {
                Debug.LogError("Target is not assigned in EnemyPathfinding!");
                enabled = false;
                return;
            }
            nodeRadius = config.NodeRadius;
            timer = updatePathInterval;
        }

        private void Update()
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = updatePathInterval;
                RequestPath();
            }
        }

        private void RequestPath()
        {
            PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
        }

        private void OnPathFound(List<Vector3> newPath)
        {
            if (newPath == null || newPath.Count == 0)
            {
                Debug.LogWarning("No path found.");
                return;
            }
            path = newPath;
            targetIndex = 0;
            FollowPath();
        }

        private void FollowPath()
        {
            if (path == null || path.Count == 0) return;

            Vector3 currentWaypoint = path[targetIndex];
            if (Vector3.Distance(transform.position, currentWaypoint) < stoppingDistance)
            {
                targetIndex++;
                if (targetIndex >= path.Count)
                {
                    path = null;
                    return;
                }
                currentWaypoint = path[targetIndex];
            }

            //Vector3 direction = (currentWaypoint - transform.position).normalized;
            //enemyMovement.Move(direction); // Remove this line
        }

        private void OnDrawGizmos()
        {
            if (!showGizmos) return;

            if (config == null) return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, config.NodeRadius);

            if (path != null)
            {
                for (int i = targetIndex; i < path.Count; i++)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawCube(path[i], Vector3.one * (nodeRadius * 0.5f));

                    if (i == targetIndex)
                    {
                        Gizmos.DrawLine(transform.position, path[i]);
                    }
                    else
                    {
                        Gizmos.DrawLine(path[i - 1], path[i]);
                    }
                }
            }
        }

        public void UpdatePath()
        {
            RequestPath();
        }
    }
}
