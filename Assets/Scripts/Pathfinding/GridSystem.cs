using UnityEngine;
using System.Collections.Generic;
using Core.Pathfinding;
using Core.Pooling;

namespace Core.Pathfinding
{
    public class GridSystem
    {
        private readonly PathfindingConfig config;
        private readonly Vector2Int gridSize;
        private readonly float nodeRadius;
        private readonly PathNode[,] grid; // Changed to PathNode[,]
        private readonly Vector3 worldBottomLeft;
        private readonly ObjectPool<PathNode> nodePool;
        private readonly Queue<PathNode> openSet; // Changed to Queue<PathNode>
        private readonly HashSet<PathNode> closedSet; // Changed to HashSet<PathNode>

        public GridSystem(PathfindingConfig config)
        {
            this.config = config ?? throw new System.ArgumentNullException(nameof(config));
            this.nodeRadius = config.nodeRadius;

            gridSize = CalculateGridSize();
            worldBottomLeft = CalculateWorldBottomLeft();
            grid = CreateGrid();

            nodePool = new ObjectPool<PathNode>(CreatePathNode, null, 100);
            openSet = new Queue<PathNode>(); // Initialize as Queue<PathNode>
            closedSet = new HashSet<PathNode>(); // Initialize as HashSet<PathNode>
        }

        private Vector2Int CalculateGridSize()
        {
            return new Vector2Int(
                Mathf.RoundToInt(config.gridWorldSize.x / (nodeRadius * 2)),
                Mathf.RoundToInt(config.gridWorldSize.y / (nodeRadius * 2))
            );
        }

        private Vector3 CalculateWorldBottomLeft()
        {
            return Vector3.zero -
                   Vector3.right * config.gridWorldSize.x / 2 -
                   Vector3.up * config.gridWorldSize.y / 2;
        }

        private PathNode[,] CreateGrid() // Changed to return PathNode[,]
        {
            var newGrid = new PathNode[gridSize.x, gridSize.y]; // Changed to PathNode[,]

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    Vector3 worldPoint = CalculateWorldPoint(x, y);
                    bool walkable = !Physics2D.CircleCast(worldPoint, nodeRadius, Vector2.zero, 0, config.unwalkableMask);
                    // Create PathNode instead of Node
                    newGrid[x, y] = nodePool.Get();
                    newGrid[x, y].Initialize(walkable, worldPoint, x, y);
                }
            }

            return newGrid;
        }

        private Vector3 CalculateWorldPoint(int x, int y)
        {
            return worldBottomLeft +
                   Vector3.right * (x * nodeRadius * 2 + nodeRadius) +
                   Vector3.up * (y * nodeRadius * 2 + nodeRadius);
        }

        private PathNode CreatePathNode()
        {
            return new PathNode();
        }

        public List<Vector3> FindPath(Vector3 startPos, Vector3 targetPos)
        {
            try
            {
                openSet.Clear();
                closedSet.Clear();

                PathNode startNode = NodeFromWorldPoint(startPos);
                PathNode targetNode = NodeFromWorldPoint(targetPos);

                if (startNode == null || targetNode == null || !startNode.Walkable || !targetNode.Walkable)
                    return new List<Vector3>();

                return AStar.FindPath(startNode, targetNode, grid, openSet, closedSet); // Modified to pass openSet and closedSet
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Path finding error: {e.Message}");
                return new List<Vector3>();
            }
        }

        private PathNode NodeFromWorldPoint(Vector3 worldPosition) // Changed to return PathNode
        {
            float percentX = (worldPosition.x + config.gridWorldSize.x / 2) / config.gridWorldSize.x;
            float percentY = (worldPosition.y + config.gridWorldSize.y / 2) / config.gridWorldSize.y;

            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            int x = Mathf.RoundToInt((gridSize.x - 1) * percentX);
            int y = Mathf.RoundToInt((gridSize.y - 1) * percentY);

            return grid[x, y];
        }
    }
}
