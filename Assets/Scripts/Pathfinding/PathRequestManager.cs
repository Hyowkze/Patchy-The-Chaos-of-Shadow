using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Pathfinding;

namespace Core.Pathfinding
{
    public class PathRequestManager : MonoBehaviour
    {
        private static PathRequestManager instance;
        private Queue<PathRequest> pathRequestQueue;
        private PathRequest currentRequest;
        private bool isProcessingPath;
        private GridSystem gridSystem; // Add a reference to GridSystem

        [SerializeField] private PathfindingConfig config; // Add a field to hold the PathfindingConfig

        private void Awake()
        {
            instance = this;
            pathRequestQueue = new Queue<PathRequest>();
            // Initialize GridSystem here, after the config has been assigned
            if (config == null)
            {
                Debug.LogError("PathfindingConfig is not assigned in PathRequestManager!");
                enabled = false;
                return;
            }
            gridSystem = new GridSystem(config);
        }

        public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, System.Action<List<Vector3>> callback)
        {
            PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
            instance.pathRequestQueue.Enqueue(newRequest);
            instance.TryProcessNext();
        }

        private void TryProcessNext()
        {
            if (isProcessingPath || pathRequestQueue.Count == 0) return;

            currentRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            ProcessPathRequest();
        }

        private async void ProcessPathRequest()
        {
            List<Vector3> waypoints = await Task.Run(() =>
                gridSystem.FindPath(currentRequest.pathStart, currentRequest.pathEnd) // Use the instance of GridSystem
            );

            FinishProcessingPath(waypoints);
        }

        private void FinishProcessingPath(List<Vector3> path)
        {
            currentRequest.callback(path);
            isProcessingPath = false;
            TryProcessNext();
        }

        private struct PathRequest
        {
            public Vector3 pathStart;
            public Vector3 pathEnd;
            public System.Action<List<Vector3>> callback;

            public PathRequest(Vector3 start, Vector3 end, System.Action<List<Vector3>> callback)
            {
                this.pathStart = start;
                this.pathEnd = end;
                this.callback = callback;
            }
        }
    }
}
