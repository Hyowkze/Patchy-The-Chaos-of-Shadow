using UnityEngine;
using System.Collections.Generic;

public class GridSystem
{
    private readonly PathfindingConfig config;
    private readonly Vector2Int gridSize;
    private readonly float nodeRadius;
    private readonly Node[,] grid;
    private readonly Vector3 worldBottomLeft;

    public GridSystem(PathfindingConfig config)
    {
        this.config = config ?? throw new System.ArgumentNullException(nameof(config));
        this.nodeRadius = config.nodeRadius;
        
        gridSize = CalculateGridSize();
        worldBottomLeft = CalculateWorldBottomLeft();
        grid = CreateGrid();
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
               Vector3.right * config.gridWorldSize.x/2 - 
               Vector3.up * config.gridWorldSize.y/2;
    }

    private Node[,] CreateGrid()
    {
        var newGrid = new Node[gridSize.x, gridSize.y];

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 worldPoint = CalculateWorldPoint(x, y);
                bool walkable = !Physics2D.CircleCast(worldPoint, nodeRadius, Vector2.zero, 0, config.unwalkableMask);
                newGrid[x,y] = new Node(walkable, worldPoint, x, y);
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

    public List<Vector3> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = NodeFromWorldPoint(startPos);
        Node targetNode = NodeFromWorldPoint(targetPos);
        
        if (startNode == null || targetNode == null) 
            return new List<Vector3>();

        return AStar.FindPath(startNode, targetNode, grid);
    }

    private Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + config.gridWorldSize.x/2) / config.gridWorldSize.x;
        float percentY = (worldPosition.y + config.gridWorldSize.y/2) / config.gridWorldSize.y;
        
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSize.x - 1) * percentX);
        int y = Mathf.RoundToInt((gridSize.y - 1) * percentY);
        
        return grid[x,y];
    }
}

public class Node
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;
    public int gCost;
    public int hCost;
    public Node parent;

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }

    public int fCost => gCost + hCost;
}

public static class AStar
{
    public static List<Vector3> FindPath(Node startNode, Node targetNode, Node[,] grid)
    {
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || 
                    openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbour in GetNeighbours(currentNode, grid))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                    continue;

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        return new List<Vector3>();
    }

    private static List<Vector3> RetracePath(Node startNode, Node endNode)
    {
        List<Vector3> path = new List<Vector3>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.worldPosition);
            currentNode = currentNode.parent;
        }
        path.Add(startNode.worldPosition);
        path.Reverse();
        return path;
    }

    private static List<Node> GetNeighbours(Node node, Node[,] grid)
    {
        List<Node> neighbours = new List<Node>();
        int gridSizeX = grid.GetLength(0);
        int gridSizeY = grid.GetLength(1);

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    private static int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
