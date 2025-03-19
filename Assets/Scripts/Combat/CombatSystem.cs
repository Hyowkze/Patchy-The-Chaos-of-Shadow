using UnityEngine;
using System.Collections;
using Core.Combat;
using Core.Characters;
using Player.Input; // <--- Added this using directive

namespace Core.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatSystem : MonoBehaviour, IAttacker, IAttackable, IDefendable
    {
        private const float ATTACK_RESET_DELAY = 0.2f;

        [Header("Combat Stats")]
        [SerializeField] private AttackConfig attackConfig;
        [SerializeField] private LayerMask AttackableLayer;
        [SerializeField] private float specialAttackCooldown = 3f;
        private IAttackStrategy attackStrategy;
        private IAttackStrategy specialAttackStrategy;
        private IAttackStrategy currentAttackStrategy;
        private Vector2 attackDirection = Vector2.right;

        [Header("Defense Stats")]
        [SerializeField] private DefenseConfig defenseConfig;
        [SerializeField] private bool isInvulnerable = false;
        private IDefenseStrategy defenseStrategy;
        private IDefenseStrategy currentDefenseStrategy;

        [Header("References")]
        [SerializeField] private Transform attackOrigin;

        private Health healthComponent;
        private PlayerInputHandler inputHandler;
        private bool canPerformSpecialAttack = true;
        private float specialAttackTimer;

        public bool IsAttacking { get; private set; }
        public bool IsInvulnerable => isInvulnerable;
        public float Defense => defenseConfig.defense;

        public IAttackStrategy BasicAttackStrategy => attackStrategy;
        public IAttackStrategy SpecialAttackStrategy => specialAttackStrategy;

        public event System.Action<bool> OnAttackStateChanged;

        private void Awake()
        {
            InitializeComponents();
            InitializeStrategies();
        }

        private void Start()
        {
            if (inputHandler != null)
            {
                inputHandler.OnAttackInputChanged += HandleAttackInput;
                inputHandler.OnSpecialInputChanged += HandleSpecialInput;
            }
            else
            {
                Debug.LogError("Input Handler is null");
            }
        }


        private void InitializeComponents()
        {
            healthComponent = GetComponent<Health>();
            inputHandler = PlayerInputHandler.Instance;

            if (attackOrigin == null)
            {
                attackOrigin = transform;
                Debug.LogWarning($"Attack origin not set on {gameObject.name}. Using transform as default.");
            }
        }

        private void InitializeStrategies()
        {
            attackStrategy = new BasicAttackStrategy(attackConfig);
            specialAttackStrategy = new RaycastAttackStrategy(attackConfig.specialDamage, attackConfig.shootRange);
            currentAttackStrategy = attackStrategy;
            defenseStrategy = new BasicDefenseStrategy(defenseConfig);
            currentDefenseStrategy = defenseStrategy;
        }

        private void Update()
        {
            if (!canPerformSpecialAttack)
            {
                specialAttackTimer -= Time.deltaTime;
                if (specialAttackTimer <= 0)
                {
                    canPerformSpecialAttack = true;
                }
            }
        }

        private void HandleAttackInput()
        {
            PerformAttack();
        }

        private void HandleSpecialInput()
        {
            if (canPerformSpecialAttack)
            {
                PerformSpecialAttack();
            }
        }

        public void PerformAttack()
        {
            ExecuteAttack(currentAttackStrategy);
        }

        private void PerformSpecialAttack()
        {
            currentAttackStrategy = specialAttackStrategy;
            ExecuteAttack(currentAttackStrategy);
            currentAttackStrategy = attackStrategy;
            canPerformSpecialAttack = false;
            specialAttackTimer = specialAttackCooldown;
        }

        private void ExecuteAttack(IAttackStrategy strategy)
        {
            if (strategy == null) return;

            IsAttacking = true;
            OnAttackStateChanged?.Invoke(true);

            float facingDirection = transform.localScale.x;
            attackDirection = new Vector2(Mathf.Sign(facingDirection), 0f);

            strategy.Execute(attackOrigin.position, attackDirection, AttackableLayer);

            StartCoroutine(ResetAttackState());
        }

        private IEnumerator ResetAttackState()
        {
            yield return new WaitForSeconds(ATTACK_RESET_DELAY);
            IsAttacking = false;
            OnAttackStateChanged?.Invoke(false);
        }

        public void TakeDamage(float amount)
        {
            if (isInvulnerable) return;
            if (healthComponent == null)
            {
                Debug.LogError("Health component is null");
                return;
            }
            ApplyDefense(amount);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, attackDirection * attackConfig.shootRange);
        }

        void IAttacker.PerformSpecialAttack()
        {
            PerformSpecialAttack();
        }

        public void ApplyDefense(float incomingDamage)
        {
            if (!isInvulnerable)
            {
                float finalDamage = currentDefenseStrategy.CalculateDefense(incomingDamage); // Modified: Use currentDefenseStrategy
                healthComponent?.TakeDamage(finalDamage);
            }
        }

        private void OnDestroy()
        {
            if (inputHandler != null)
            {
                inputHandler.OnAttackInputChanged -= HandleAttackInput;
                inputHandler.OnSpecialInputChanged -= HandleSpecialInput;
            }
        }

        private void OnDisable()
        {
            if (inputHandler != null)
            {
                inputHandler.OnAttackInputChanged -= HandleAttackInput;
                inputHandler.OnSpecialInputChanged -= HandleSpecialInput;
            }
        }

        public void SetCurrentAttackStrategy(IAttackStrategy strategy)
        {
            currentAttackStrategy = strategy;
        }

        public void SetCurrentDefenseStrategy(IDefenseStrategy strategy)
        {
            currentDefenseStrategy = strategy;
        }
    }
}
