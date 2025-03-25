using UnityEngine;
using Core.Pooling;
using Core.Combat.Projectiles;

namespace Core.Combat
{
    public class HomingAttackStrategy : IAttackStrategy
    {
        private readonly AttackConfig config;

        public HomingAttackStrategy(AttackConfig config)
        {
            this.config = config;
        }

        public void Execute(Vector2 origin, Vector2 direction, LayerMask targetLayer)
        {
            GameObject projectile = ObjectPool.Instance.SpawnFromPool(
                config.specialAttackPoolTag, 
                origin, 
                Quaternion.identity);

            if (projectile != null && projectile.TryGetComponent<HomingProjectile>(out var homingProjectile))
            {
                homingProjectile.Initialize(
                    config.specialDamage,
                    config.specialProjectileSpeed,
                    config.specialShootRange,
                    direction,
                    targetLayer);
            }
        }
    }
}
