using System.Collections.Generic;
using UnityEngine;

namespace Core.Pathfinding
{
    public static class AStar
    {
        public static List<Vector3> FindPath(PathNode startNode, PathNode targetNode, PathNode[,] grid, Queue<PathNode> openSet, HashSet<PathNode> closedSet)
        {
            openSet.Enqueue(startNode);
            startNode.GCost = 0;
            startNode.HCost = GetDistance(startNode, targetNode);

            while (openSet.Count > 0)
            {
                PathNode currentNode = openSet.Dequeue();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    return RetracePath(startNode, targetNode);
                }

                foreach (PathNode neighbor in GetNeighbors(currentNode, grid))
                {
                    if (!neighbor.Walkable || closedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbor = currentNode.GCost + GetDistance(currentNode, neighbor);
                    if (newMovementCostToNeighbor < neighbor.GCost || !openSet.Contains(neighbor))
                    {
                        neighbor.GCost = newMovementCostToNeighbor;
                        neighbor.HCost = GetDistance(neighbor, targetNode);
                        neighbor.Parent = currentNode;

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Enqueue(neighbor);
                        }
                    }
                }
            }

            return new List<Vector3>();
        }

        private static List<Vector3> RetracePath(PathNode startNode, PathNode endNode)
        {
            List<Vector3> path = new List<Vector3>();
            PathNode currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode.WorldPosition);
                currentNode = (PathNode)currentNode.Parent;
            }
            path.Add(startNode.WorldPosition);
            path.Reverse();
            return path;
        }

        private static List<PathNode> GetNeighbors(PathNode node, PathNode[,] grid)
        {
            List<PathNode> neighbors = new List<PathNode>();
            int gridSizeX = grid.GetLength(0);
            int gridSizeY = grid.GetLength(1);

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;

                    int checkX = node.GridX + x;
                    int checkY = node.GridY + y;

                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    {
                        neighbors.Add(grid[checkX, checkY]);
                    }
                }
            }

            return neighbors;
        }

        private static int GetDistance(PathNode nodeA, PathNode nodeB)
        {
            int dstX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
            int dstY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }
    }
}
