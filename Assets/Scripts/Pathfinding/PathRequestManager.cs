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

        [SerializeField] private PathfindingConfig config; // Add a field to hold the PathfindingConfig

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
                return;
            }
            instance = this;
            pathRequestQueue = new Queue<PathRequest>();
            // Validate PathfindingConfig
            if (config == null)
            {
                Debug.LogError("PathfindingConfig is not assigned in PathRequestManager!");
                enabled = false;
                return;
            }
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
            // Get the start and end nodes from the world positions
            PathNode startNode = GridSystem.Instance.NodeFromWorldPoint(currentRequest.pathStart);
            PathNode endNode = GridSystem.Instance.NodeFromWorldPoint(currentRequest.pathEnd);

            // Check if startNode or endNode is null
            if (startNode == null || endNode == null)
            {
                Debug.LogError($"Start or End node is null. Start: {startNode}, End: {endNode}");
                FinishProcessingPath(new List<Vector3>()); // Return an empty path
                return;
            }

            List<Vector3> waypoints = await Task.Run(() =>
                AStar.FindPath(startNode, endNode) // Correct call to AStar.FindPath
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