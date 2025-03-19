using UnityEngine;
using Core.Utils;

namespace Core.Utils
{
    public static class GridUtils
    {
        private static readonly RaycastHit2D[] rayResults = new RaycastHit2D[1];
        //private static readonly Collider2D[] colliderResults = new Collider2D[1]; // No longer needed

        public static bool IsValidPosition(Vector3 worldPoint, LayerMask groundLayer, LayerMask obstacleLayer, float checkRadius = 0.5f)
        {
            if (Physics2D.RaycastNonAlloc(worldPoint + Vector3.up, Vector2.down, rayResults, 2f, groundLayer) == 0)
                return false;

            //return Physics2D.OverlapCircleNonAlloc(worldPoint, checkRadius, colliderResults, obstacleLayer) == 0; // Old line
            return Physics2D.OverlapCircle(worldPoint, checkRadius, obstacleLayer) == null; // New line
        }

        public static int GetManhattanDistance(Vector2Int posA, Vector2Int posB)
        {
            return Mathf.Abs(posA.x - posB.x) + Mathf.Abs(posA.y - posB.y);
        }

        public static Vector2 GetClosestPointOnGrid(Vector2 position, float gridSize)
        {
            float x = Mathf.Round(position.x / gridSize) * gridSize;
            float y = Mathf.Round(position.y / gridSize) * gridSize;
            return new Vector2(x, y);
        }
    }
}
