using UnityEngine;

[CreateAssetMenu(fileName = "PhysicsConfig", menuName = "Config/PhysicsConfig")]
public class PhysicsConfig : ScriptableObject
{
    [Header("Collision Detection")]
    public float WallCollisionAngleThreshold = 45f;
    public float GroundCollisionAngleThreshold = 45f;
}
