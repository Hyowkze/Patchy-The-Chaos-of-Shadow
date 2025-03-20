using UnityEngine;
using System.Collections.Generic;
using Core.Utils;

namespace Core.Pathfinding
{
    public class GridSystem : MonoBehaviour
    {
        public static GridSystem Instance { get; private set; } // Singleton instance

        [SerializeField] private PathfindingConfig config;
        [SerializeField] private bool showGridGizmos = true; // Add this
        private Vector2Int gridSize;
        private float nodeRadius;
        public PathNode[,] grid; // Make grid public

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            //DontDestroyOnLoad(gameObject); // If you need the grid to persist between scenes

            nodeRadius = config.nodeRadius;
            gridSize = new Vector2Int(Mathf.RoundToInt(config.gridWorldSize.x / (nodeRadius * 2)), Mathf.RoundToInt(config.gridWorldSize.y / (nodeRadius * 2)));
            grid = CreateGrid();
        }

        private PathNode[,] CreateGrid()
        {
            var newGrid = new PathNode[gridSize.x, gridSize.y];

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    Vector3 worldPoint = CalculateWorldPoint(x, y);
                    bool walkable = !Physics2D.CircleCast(worldPoint, nodeRadius, Vector2.zero, 0, config.unwalkableMask);
                    newGrid[x, y] = new PathNode(walkable, worldPoint, x, y);
                }
            }

            return newGrid;
        }

        public List<PathNode> GetNeighbours(PathNode node)
        {
            if (grid == null) return new List<PathNode>(); // Add this line

            List<PathNode> neighbours = new List<PathNode>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    int checkX = node.GridX + x;
                    int checkY = node.GridY + y;

                    if (checkX >= 0 && checkX < gridSize.x && checkY >= 0 && checkY < gridSize.y)
                    {
                        neighbours.Add(grid[checkX, checkY]);
                    }
                }
            }

            return neighbours;
        }

        private Vector3 CalculateWorldPoint(int x, int y)
        {
            Vector3 worldBottomLeft = transform.position - Vector3.right * config.gridWorldSize.x / 2 - Vector3.up * config.gridWorldSize.y / 2;
            return worldBottomLeft + Vector3.right * (x * nodeRadius * 2 + nodeRadius) + Vector3.up * (y * nodeRadius * 2 + nodeRadius);
        }

        public PathNode NodeFromWorldPoint(Vector3 worldPosition)
        {
            if (grid == null) return null; // Add this line

            float percentX = Mathf.Clamp01((worldPosition.x + config.gridWorldSize.x / 2) / config.gridWorldSize.x);
            float percentY = Mathf.Clamp01((worldPosition.y + config.gridWorldSize.y / 2) / config.gridWorldSize.y);

            int x = Mathf.RoundToInt((gridSize.x - 1) * percentX);
            int y = Mathf.RoundToInt((gridSize.y - 1) * percentY);

            return grid[x, y];
        }

        private void OnDrawGizmos()
        {
            if (!showGridGizmos || grid == null) return; // Check the flag

            if (!Application.isEditor) return; // Add this line

            foreach (PathNode n in grid)
            {
                Gizmos.color = n.Walkable ? Color.white : Color.red;
                Gizmos.DrawCube(n.WorldPosition, Vector3.one * (nodeRadius * 2 - 0.1f));
            }
        }
    }
}
