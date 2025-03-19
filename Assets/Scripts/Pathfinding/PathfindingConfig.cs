using UnityEngine;

namespace Core.Pathfinding
{
    [CreateAssetMenu(fileName = "PathfindingConfig", menuName = "Config/PathfindingConfig")]
    public class PathfindingConfig : ScriptableObject
    {
        [Header("Grid Settings")]
        [Min(1f)] public Vector2 gridWorldSize = new Vector2(50f, 50f);
        [Min(0.1f)] public float nodeRadius = 0.5f;
        
        [Header("Detection Settings")]
        public LayerMask unwalkableMask;
        [Min(0.1f)] public float pathUpdateRate = 0.5f;

        private void OnValidate()
        {
            gridWorldSize.x = Mathf.Max(1f, gridWorldSize.x);
            gridWorldSize.y = Mathf.Max(1f, gridWorldSize.y);
            nodeRadius = Mathf.Max(0.1f, nodeRadius);
            pathUpdateRate = Mathf.Max(0.1f, pathUpdateRate);
        }
    }
}
