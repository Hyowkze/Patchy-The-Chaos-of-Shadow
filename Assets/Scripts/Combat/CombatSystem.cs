using UnityEngine;
using Core.Characters;
using Core.Pooling;
using Core.Combat.Projectiles;

namespace Core.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatSystem : MonoBehaviour, IAttackable, IAttacker
    {
        [Header("Attack Settings")]
        [SerializeField] private AttackConfig attackConfig;
        [SerializeField] private LayerMask targetLayer;
        [SerializeField] private Transform attackOrigin;

        [Header("Defense Settings")]
        [SerializeField] private DefenseConfig defenseConfig;

        private IAttackStrategy currentAttackStrategy;
        private IDefenseStrategy currentDefenseStrategy;
        private Health health;
        private bool isAttacking;

        public IAttackStrategy BasicAttackStrategy { get; private set; }
        public IAttackStrategy SpecialAttackStrategy { get; private set; }
        public AttackConfig AttackConfig => attackConfig;

        public event System.Action<bool> OnAttackStateChanged;

        private void Awake()
        {
            health = GetComponent<Health>();
            BasicAttackStrategy = new BasicAttackStrategy(attackConfig);
            SpecialAttackStrategy = new HomingAttackStrategy(attackConfig); // Cambiar a HomingAttackStrategy
            currentDefenseStrategy = new BasicDefenseStrategy(defenseConfig);
            currentAttackStrategy = BasicAttackStrategy;
        }

        private void Start()
        {
            InitializeAttackPools();
        }

        private void InitializeAttackPools()
        {
            if (attackConfig.basicProjectilePrefab != null)
            {
                ObjectPool.Instance.CreatePool(attackConfig.basicAttackPoolTag, 
                    attackConfig.basicProjectilePrefab, attackConfig.poolSize);
            }
            if (attackConfig.specialProjectilePrefab != null)
            {
                ObjectPool.Instance.CreatePool(attackConfig.specialAttackPoolTag, 
                    attackConfig.specialProjectilePrefab, attackConfig.poolSize);
            }
        }

        public void SetCurrentAttackStrategy(IAttackStrategy strategy)
        {
            currentAttackStrategy = strategy;
        }

        public void TakeDamage(float damage)
        {
            float finalDamage = currentDefenseStrategy.CalculateDefense(damage);
            health.TakeDamage(finalDamage);
        }

        public void PerformAttack(IAttackStrategy attackStrategy)
        {
            if (isAttacking) return;
            isAttacking = true;
            OnAttackStateChanged?.Invoke(isAttacking);
            Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
            attackStrategy.Execute(attackOrigin.position, direction, targetLayer);
            isAttacking = false;
            OnAttackStateChanged?.Invoke(isAttacking);
        }
    }

    public interface IAttackable
    {
        void TakeDamage(float damage);
    }

    public interface IAttacker
    {
        void PerformAttack(IAttackStrategy attackStrategy);
    }
}
