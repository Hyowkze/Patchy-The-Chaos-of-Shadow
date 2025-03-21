using UnityEngine;
using Core.Combat;
using Core.Player.Movement;

[CreateAssetMenu(fileName = "AttackConfig", menuName = "Combat/AttackConfig")]
public class AttackConfig : ScriptableObject
{
    public float baseDamage = 20f;
    public float specialDamage = 40f;
    public float shootRange = 10f;
    public float specialAttackCooldown = 2f;
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
