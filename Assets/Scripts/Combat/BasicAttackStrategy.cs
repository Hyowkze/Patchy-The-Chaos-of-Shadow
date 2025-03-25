using UnityEngine;
using Core.Combat;
using Core.Pooling;
using Core.Combat.Projectiles;

namespace Core.Combat
{
    public class BasicAttackStrategy : IAttackStrategy
    {
        private readonly AttackConfig config;

        public BasicAttackStrategy(AttackConfig config)
        {
            this.config = config;
        }

        public void Execute(Vector2 origin, Vector2 direction, LayerMask targetLayer)
        {
            GameObject projectile = ObjectPool.Instance.SpawnFromPool(
                config.basicAttackPoolTag, 
                origin, 
                Quaternion.identity);

            if (projectile != null && projectile.TryGetComponent<HomingProjectile>(out var homingProjectile))
            {
                homingProjectile.Initialize(
                    config.baseDamage, 
                    config.basicProjectileSpeed,
                    config.basicShootRange, 
                    direction, 
                    targetLayer);
            }
        }
    }
}
