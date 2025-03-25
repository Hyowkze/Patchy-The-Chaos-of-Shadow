using UnityEngine;
using Core.Combat;
using Core.Player.Movement;
using Core.Combat.Projectiles;

[CreateAssetMenu(fileName = "AttackConfig", menuName = "Combat/AttackConfig")]
public class AttackConfig : ScriptableObject
{
    [Header("Basic Attack Settings")]
    public float baseDamage = 20f;
    public float basicProjectileSpeed = 10f;
    public float basicShootRange = 10f;
    public string basicAttackPoolTag = "BasicAttack";
    public GameObject basicProjectilePrefab;

    [Header("Special Attack Settings")]
    public float specialDamage = 40f;
    public float specialProjectileSpeed = 15f;
    public float specialShootRange = 15f;
    public float specialAttackCooldown = 2f;
    public float aimingTime = 1f;
    public string specialAttackPoolTag = "SpecialAttack";
    public GameObject specialProjectilePrefab;

    [Header("Pool Settings")]
    public int poolSize = 5;

    [Header("Visual Settings")]
    public GameObject aimIndicatorPrefab;
}

public interface IAttackStrategy
{
    void Execute(Vector2 origin, Vector2 direction, LayerMask targetLayer);
}

public interface IMovementBehavior
{
    Vector2 CalculateVelocity(Vector2 currentVelocity, float horizontalInput);
}

public class RaycastAttackStrategy : IAttackStrategy
{
    private readonly float damage;
    private readonly float range;

    public RaycastAttackStrategy(float damage, float range)
    {
        this.damage = damage;
        this.range = range;
    }

    public void Execute(Vector2 origin, Vector2 direction, LayerMask targetLayer)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, range, targetLayer);
        
        if (hit.collider != null && hit.collider.TryGetComponent<CombatSystem>(out var targetCombat))
        {
            targetCombat.TakeDamage(damage);
        }
    }
}

public class GroundMovement : IMovementBehavior
{
    private readonly MovementConfig config;

    public GroundMovement(MovementConfig config)
    {
        this.config = config;
    }

    public Vector2 CalculateVelocity(Vector2 currentVelocity, float horizontalInput)
    {
        return new Vector2(horizontalInput * config.MoveSpeed, currentVelocity.y);
    }
}
