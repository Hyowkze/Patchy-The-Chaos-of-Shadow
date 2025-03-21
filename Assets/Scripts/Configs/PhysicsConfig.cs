using UnityEngine;

namespace Core.Player.Movement
{
    [CreateAssetMenu(fileName = "PhysicsConfig", menuName = "Configs/Physics Config", order = 2)]
    public class PhysicsConfig : ScriptableObject
    {
        [Header("Collision Settings")]
        [Range(0f, 90f)] public float WallCollisionAngleThreshold = 45f;
        [Range(0f, 90f)] public float GroundCollisionAngleThreshold = 45f;
    }
}