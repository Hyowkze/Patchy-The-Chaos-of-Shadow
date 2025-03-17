using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(EnemyDetection))]
public class EnemyPathfinding : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private PathfindingConfig config;
    
    [Header("Pathfinding Settings")]
    [SerializeField] private float pathUpdateRate = 0.5f;
    [SerializeField] private float nodeRadius = 0.5f;
    [SerializeField] private bool debugPath;
    
    private GridSystem gridSystem;
    private EnemyDetection detection;
    private readonly List<Vector3> currentPath = new List<Vector3>();
    private float pathUpdateTimer;
    
    public IReadOnlyList<Vector3> CurrentPath => currentPath;

    private void Awake()
    {
        detection = GetComponent<EnemyDetection>();
        
        if (config == null)
        {
            Debug.LogError($"Missing PathfindingConfig on {gameObject.name}");
            enabled = false;
            return;
        }

        InitializeGridSystem();
    }

    private void InitializeGridSystem()
    {
        try
        {
            gridSystem = new GridSystem(config);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to initialize GridSystem: {e.Message}");
            enabled = false;
        }
    }

    private void Start()
    {
        UpdatePath();
    }

    private void Update()
    {
        if (!detection.PlayerFound) return;
        
        pathUpdateTimer -= Time.deltaTime;
        if (pathUpdateTimer <= 0f)
        {
            UpdatePath();
            pathUpdateTimer = pathUpdateRate;
        }
    }

    public void UpdatePath()
    {
        if (!detection.PlayerFound) return;
        
        currentPath.Clear();
        currentPath.AddRange(gridSystem.FindPath(
            transform.position,
            detection.PlayerPosition
        ));
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || !debugPath) return;
        DrawPathGizmos();
    }

    private void DrawPathGizmos()
    {
        if (currentPath == null || currentPath.Count == 0) return;

        Gizmos.color = Color.blue;
        for (int i = 0; i < currentPath.Count - 1; i++)
        {
            Gizmos.DrawLine(currentPath[i], currentPath[i + 1]);
            Gizmos.DrawWireSphere(currentPath[i], nodeRadius);
        }
    }
}