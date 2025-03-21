using UnityEngine;
using System.Collections.Generic;
using Core.Utils; 

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
        Vector3 worldBottomLeft = origin - new Vector3(config.GridWorldSize.x / 2, config.GridWorldSize.y / 2);

        for (float x = 0; x < config.GridWorldSize.x; x += config.NodeSize)
        {
            for (float y = 0; y < config.GridWorldSize.y; y += config.NodeSize)
            {
                Vector3 worldPoint = worldBottomLeft + new Vector3(x, y);
                // Use GridUtils.IsValidPosition instead of the old method
                if (GridUtils.IsValidPosition(worldPoint, groundLayer, obstacleLayer))
                {
                    positions.Add(worldPoint);
                }
            }
        }

        return positions;
    }
}