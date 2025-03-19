using UnityEngine;
using System.Collections.Generic;

public interface IGridGenerator
{
    List<Vector3> GenerateGrid(Vector3 origin, LayerMask groundLayer, LayerMask obstacleLayer);
}

public class DefaultGridGenerator : IGridGenerator
{
    private readonly GridGenerationConfig config;

    public DefaultGridGenerator(GridGenerationConfig config)
    {
        this.config = config;
    }

    public List<Vector3> GenerateGrid(Vector3 origin, LayerMask groundLayer, LayerMask obstacleLayer)
    {
        List<Vector3> positions = new List<Vector3>();
        Vector3 worldBottomLeft = origin - new Vector3(config.GridWorldSize.x/2, config.GridWorldSize.y/2);

        for (float x = 0; x < config.GridWorldSize.x; x += config.NodeSize)
        {
            for (float y = 0; y < config.GridWorldSize.y; y += config.NodeSize)
            {
                Vector3 worldPoint = worldBottomLeft + new Vector3(x, y);
                if (IsValidPosition(worldPoint, groundLayer, obstacleLayer))
                {
                    positions.Add(worldPoint);
                }
            }
        }

        return positions;
    }

    private bool IsValidPosition(Vector3 worldPoint, LayerMask groundLayer, LayerMask obstacleLayer)
    {
        RaycastHit2D groundHit = Physics2D.Raycast(worldPoint + Vector3.up * 1f, Vector2.down, 2f, groundLayer);
        if (!groundHit) return false;

        Collider2D[] obstacles = Physics2D.OverlapCircleAll(worldPoint, 0.5f, obstacleLayer);
        return obstacles.Length == 0;
    }
}
