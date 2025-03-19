using UnityEngine;
using Core.Combat; // Added this using directive

public class BasicAttackStrategy : IAttackStrategy
{
    private readonly AttackConfig config;

    public BasicAttackStrategy(AttackConfig config)
    {
        this.config = config;
    }

    public void Execute(Vector2 origin, Vector2 direction, LayerMask targetLayer)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, config.shootRange, targetLayer);

        if (hit.collider != null && hit.collider.TryGetComponent<CombatSystem>(out var targetCombat))
        {
            targetCombat.TakeDamage(config.baseDamage);
        }
    }
}
