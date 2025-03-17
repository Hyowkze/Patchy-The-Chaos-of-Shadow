using UnityEngine;
using System.Collections.Generic;

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));
    }
}
