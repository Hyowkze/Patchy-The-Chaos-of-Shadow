using UnityEngine;

[CreateAssetMenu(fileName = "MovementConfig", menuName = "Config/MovementConfig")]
public class MovementConfig : ScriptableObject
{
    [Header("Basic Movement")]
    public float MoveSpeed = 5f;
    public float JumpForce = 12f;

    [Header("Dash Settings")]
    public DashSettings DashSettings;

    [Header("Sprint Settings")]
    public SprintSettings SprintSettings;

    [Header("Physics Settings")]
    public float GroundFriction = 10f;
    public float AirControl = 0.5f;
    public float CoyoteTime = 0.2f;
    public float JumpBufferTime = 0.2f;

    [Header("Movement Thresholds")]
    public float MinimumMoveSpeed = 0.1f;
    public float MaxFallSpeed = 20f;
}

[System.Serializable]
public class DashSettings
{
    public float DashSpeed = 20f;
    public float DashDuration = 0.2f;
    public float DashCooldown = 1f;
}

[System.Serializable]
public class SprintSettings
{
    public float SprintMultiplier = 1.5f;
}
