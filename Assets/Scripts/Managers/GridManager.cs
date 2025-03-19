using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private Vector2 gridWorldSize = new Vector2(100f, 100f);
    [SerializeField] private GridGenerationConfig gridConfig;

    private List<Vector3> validPositions = new List<Vector3>();
    private IGridGenerator gridGenerator;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Use the interface instead of the concrete class
            gridGenerator = new DefaultGridGenerator(gridConfig);
            GenerateValidPositions();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void GenerateValidPositions()
    {
        validPositions = gridGenerator.GenerateGrid(transform.position, groundLayer, obstacleLayer);
    }

    public Vector3 GetRandomValidPosition()
    {
        if (validPositions.Count == 0)
        {
            Debug.LogWarning("No valid positions found!");
            return Vector3.zero;
        }

        return validPositions[Random.Range(0, validPositions.Count)];
    }

    // Public accessors for grid information
    public Vector2 GridWorldSize => gridWorldSize;
    public IReadOnlyList<Vector3> ValidPositions => validPositions;

    public Vector3 GetClosestValidPosition(Vector3 position)
    {
        if (validPositions.Count == 0)
        {
            Debug.LogWarning("No valid positions found!");
            return Vector3.zero;
        }

        Vector3 closest = validPositions[0];
        float closestDistance = Vector3.Distance(position, closest);

        for (int i = 1; i < validPositions.Count; i++)
        {
            float distance = Vector3.Distance(position, validPositions[i]);
            if (distance < closestDistance)
            {
                closest = validPositions[i];
                closestDistance = distance;
            }
        }

        return closest;
    }

    public List<Vector3> GetValidPositionsInRadius(Vector3 center, float radius)
    {
        if (validPositions.Count == 0)
        {
            Debug.LogWarning("No valid positions found!");
            return new List<Vector3>();
        }

        // Filter valid positions within the radius using LINQ
        return validPositions.Where(pos => Vector3.Distance(center, pos) <= radius).ToList();
    }

    private void OnDrawGizmosSelected()
    {
        // Draw grid bounds
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));

        // Draw valid positions
        if (validPositions != null && validPositions.Count > 0)
        {
            Gizmos.color = Color.green;
            float nodeSize = gridConfig.NodeSize;
            foreach (Vector3 pos in validPositions)
            {
                Gizmos.DrawWireCube(pos, Vector3.one * nodeSize);
            }
        }
    }
}