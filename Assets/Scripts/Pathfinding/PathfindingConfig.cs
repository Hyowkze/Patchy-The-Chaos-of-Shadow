using UnityEngine;

[CreateAssetMenu(fileName = "PathfindingConfig", menuName = "Config/PathfindingConfig")]
public class PathfindingConfig : ScriptableObject
{
    public Vector2 gridWorldSize = new Vector2(50f, 50f);
    public float nodeRadius = 0.5f;
    public LayerMask unwalkableMask;
    public float pathUpdateRate = 0.5f;
}
