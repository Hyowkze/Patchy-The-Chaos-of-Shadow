using System.Collections.Generic;
using UnityEngine;
using System;

namespace Core.Pathfinding
{
    public static class AStar
    {
        public static List<Vector3> FindPath(PathNode startNode, PathNode targetNode)
        {
            SortedSet<PathNode> openSet = new SortedSet<PathNode>();
            HashSet<PathNode> closedSet = new HashSet<PathNode>();
            openSet.Add(startNode);
            startNode.GCost = 0;
            startNode.HCost = GetDistance(startNode, targetNode);

            while (openSet.Count > 0)
            {
                PathNode currentNode = openSet.Min;
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    return RetracePath(startNode, targetNode);
                }

                foreach (PathNode neighbor in GridSystem.Instance.GetNeighbours(currentNode)) // Use GridSystem.Instance
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
                            openSet.Add(neighbor);
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