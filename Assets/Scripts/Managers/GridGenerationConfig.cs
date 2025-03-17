using UnityEngine;

[CreateAssetMenu(fileName = "GridConfig", menuName = "Config/GridGenerationConfig")]
public class GridGenerationConfig : ScriptableObject
{
    public Vector2 GridWorldSize = new Vector2(100f, 100f);
    public float NodeSize = 1f;
}
