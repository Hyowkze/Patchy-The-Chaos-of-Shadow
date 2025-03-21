using UnityEngine;
using Core.Config;

namespace Core.Enemy
{
    [CreateAssetMenu(fileName = "PathfindingConfig", menuName = "Config/PathfindingConfig")]
    public class PathfindingConfig : ConfigBase
    {
        [Header("Grid Settings")]
        [SerializeField] private float gridSize = 1f;
        [SerializeField] private Vector2Int gridDimensions = new Vector2Int(10, 10);
        [SerializeField] private float nodeRadius = 0.5f;
        [SerializeField] public LayerMask unwalkableMask; // Keep this line

        public float GridSize => gridSize;
        public Vector2Int GridDimensions => gridDimensions;
        public float NodeRadius => nodeRadius;

        protected override void ValidateFields()
        {
            ValidateGreaterThanZero(ref gridSize, nameof(gridSize));
            ValidateGreaterThanZero(ref nodeRadius, nameof(nodeRadius));
        }
    }
}
