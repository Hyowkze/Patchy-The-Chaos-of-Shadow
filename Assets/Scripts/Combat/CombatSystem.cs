using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Health))]
public class CombatSystem : MonoBehaviour, IAttacker, IAttackable, IDefendable
{
    private const float ATTACK_RESET_DELAY = 0.2f;

    [Header("Combat Stats")]
    [SerializeField] private AttackConfig attackConfig;
    [SerializeField] private LayerMask AttackableLayer;
    private IAttackStrategy attackStrategy;
    private IAttackStrategy SpecialAttackStrategy;
    private Vector2 attackDirection = Vector2.right;

    [Header("Defense Stats")]
    [SerializeField] private DefenseConfig defenseConfig;
    [SerializeField] private bool isInvulnerable = false;
    private IDefenseStrategy defenseStrategy;

    private Health healthComponent;
    private PlayerInputHandler inputHandler;
    private bool canPerformSpecialAttack = true;
    private float specialAttackTimer;

    public bool IsAttacking { get; private set; }

    public bool IsInvulnerable => isInvulnerable;
    public float Defense => defenseConfig.defense;

    public event System.Action<bool> OnAttackStateChanged;

    private void Awake()
    {
        healthComponent = GetComponent<Health>();
        inputHandler = PlayerInputHandler.Instance;
        inputHandler.OnAttackInputChanged += HandleAttackInput;
        inputHandler.OnSpecialInputChanged += HandleSpecialInput;
        attackStrategy = new BasicAttackStrategy(attackConfig);
        SpecialAttackStrategy = new RaycastAttackStrategy(attackConfig.specialDamage, attackConfig.shootRange);
        defenseStrategy = new BasicDefenseStrategy(defenseConfig);
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
        ExecuteAttack(attackStrategy);
    }

    private void PerformSpecialAttack()
    {
        ExecuteAttack(SpecialAttackStrategy);
        canPerformSpecialAttack = false;
        specialAttackTimer = attackConfig.specialAttackCooldown;
    }

    private void ExecuteAttack(IAttackStrategy strategy)
    {
        IsAttacking = true;
        OnAttackStateChanged?.Invoke(true);
        
        float facingDirection = transform.localScale.x;
        attackDirection = new Vector2(Mathf.Sign(facingDirection), 0f);

        strategy.Execute(transform.position, attackDirection, AttackableLayer);

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

        float finalDamage = Mathf.Max(0, amount - defenseConfig.defense);
        healthComponent?.TakeDamage(finalDamage);
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
            float finalDamage = defenseStrategy.CalculateDefense(incomingDamage);
            healthComponent?.TakeDamage(finalDamage);
        }
    }

    private void OnDestroy()
    {
        inputHandler.OnAttackInputChanged -= HandleAttackInput;
        inputHandler.OnSpecialInputChanged -= HandleSpecialInput;
    }
    
    private void OnDisable()
    {
        inputHandler.OnAttackInputChanged -= HandleAttackInput;
        inputHandler.OnSpecialInputChanged -= HandleSpecialInput;
    }

    private class BasicAttackStrategy : IAttackStrategy
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
}
