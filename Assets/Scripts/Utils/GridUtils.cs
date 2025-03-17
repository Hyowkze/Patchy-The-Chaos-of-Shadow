using UnityEngine;

public static class GridUtils
{
    public static bool IsValidPosition(Vector3 worldPoint, LayerMask groundLayer, LayerMask obstacleLayer, float checkRadius = 0.5f)
    {
        RaycastHit2D groundHit = Physics2D.Raycast(worldPoint + Vector3.up * 1f, Vector2.down, 2f, groundLayer);
        if (!groundHit) return false;

        Collider2D[] obstacles = Physics2D.OverlapCircleAll(worldPoint, checkRadius, obstacleLayer);
        return obstacles.Length == 0;
    }

    public static int GetManhattanDistance(Vector2Int posA, Vector2Int posB)
    {
        return Mathf.Abs(posA.x - posB.x) + Mathf.Abs(posA.y - posB.y);
    }
}
