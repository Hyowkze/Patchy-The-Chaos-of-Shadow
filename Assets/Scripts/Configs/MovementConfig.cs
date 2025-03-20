using UnityEngine;

namespace Core.Player.Movement
{
    [CreateAssetMenu(fileName = "MovementConfig", menuName = "Configs/Movement Config", order = 1)]
    public class MovementConfig : ScriptableObject
    {
        [Header("Basic Movement")]
        public float MoveSpeed = 5f;
        public float JumpForce = 10f;
        public float GroundFriction = 10f;
        public float AirControl = 5f;

        [Header("Dash Settings")]
        public DashSettings DashSettings;

        [Header("Sprint Settings")]
        public SprintSettings SprintSettings;
    }

    [System.Serializable]
    public struct DashSettings
    {
        public float DashSpeed;
        public float DashDuration;
        public float DashCooldown;
    }

    [System.Serializable]
    public struct SprintSettings
    {
        public float SprintMultiplier;
        public float SprintDrainRate;
        public float SprintRechargeRate;
    }
}
