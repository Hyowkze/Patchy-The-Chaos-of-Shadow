using UnityEngine;
using Core.Enemy;
using Core.Combat;
using Core.Characters;
using Core.Player;

namespace Core.Enemy.AI
{
    [RequireComponent(typeof(EnemyMovement))]
    [RequireComponent(typeof(EnemyPathfinding))]
    [RequireComponent(typeof(EnemyDetection))]
    [RequireComponent(typeof(CombatSystem))]
    [RequireComponent(typeof(Health))]
    public class EnemyAIStateMachine : MonoBehaviour
    {
        public enum AIState
        {
            Patrolling,
            Chasing,
            Fleeing,
            PreparingAttack,
            Attacking
        }

        public AIState CurrentState { get; private set; }

        private EnemyMovement enemyMovement;
        private EnemyPathfinding enemyPathfinding;
        private EnemyDetection enemyDetection;
        private CombatSystem combatSystem;
<<<<<<< Updated upstream
        private Health health; // Added

        private Health health;
        private IAttackStrategy currentAttackStrategy;

        [Header("Flee Settings")]
        [SerializeField] private float fleeHealthThreshold = 0.3f;
        [SerializeField] private float fleeDuration = 3f;
        private float fleeTimer = 0f;

        [Header("Attack Settings")]
        [SerializeField] private float closeAttackRange = 1f;
        [SerializeField] private float midAttackRange = 2f;

        [Header("Experience Settings")]
        [SerializeField] private EnemyExperienceConfig experienceConfig; // Referencia al Scriptable Object

        private void Awake()
        {
            InitializeComponents();
            ValidateComponents();
        }

        private void InitializeComponents()
        {
            enemyMovement = GetComponent<EnemyMovement>();
            enemyPathfinding = GetComponent<EnemyPathfinding>();
            enemyDetection = GetComponent<EnemyDetection>();
            combatSystem = GetComponent<CombatSystem>();
            health = GetComponent<Health>();
        }

        private void ValidateComponents()
        {
            if (health == null || enemyMovement == null || enemyPathfinding == null ||
                enemyDetection == null || combatSystem == null)
            {
                Debug.LogError($"Missing required components on {gameObject.name}");
                enabled = false;
            }
            if (experienceConfig == null)
            {
                Debug.LogError($"Missing EnemyExperienceConfig on {gameObject.name}");
                enabled = false;
            }
        }

        private void Start()
        {
            ChangeState(AIState.Patrolling);
            health.OnDeath += HandleDeath; // Suscribirse al evento de muerte
        }

        private void Update()
        {
            switch (CurrentState)
            {
                case AIState.Patrolling:
                    PatrolUpdate();
                    break;
                case AIState.Chasing:
                    ChaseUpdate();
                    break;
                case AIState.Fleeing:
                    FleeUpdate();
                    break;
                case AIState.PreparingAttack:
                    PrepareAttackUpdate();
                    break;
                case AIState.Attacking:
                    AttackUpdate();
                    break;
            }
        }

        public void ChangeState(AIState newState)
        {
            if (CurrentState == newState) return;

            ExitState(CurrentState);
            CurrentState = newState;
            EnterState(newState);
        }

        private void EnterState(AIState state)
        {
            switch (state)
            {
                case AIState.Patrolling:
                    Debug.Log("Entering Patrolling State");
                    break;
                case AIState.Chasing:
                    Debug.Log("Entering Chasing State");
                    break;
                case AIState.Fleeing:
                    Debug.Log("Entering Fleeing State");
                    fleeTimer = fleeDuration;
                    break;
                case AIState.PreparingAttack:
                    Debug.Log("Entering PreparingAttack State");
                    break;
                case AIState.Attacking:
                    Debug.Log("Entering Attacking State");
                    break;
            }
        }

        private void ExitState(AIState state)
        {
            switch (state)
            {
                case AIState.Patrolling:
                    Debug.Log("Exiting Patrolling State");
                    break;
                case AIState.Chasing:
                    Debug.Log("Exiting Chasing State");
                    break;
                case AIState.Fleeing:
                    Debug.Log("Exiting Fleeing State");
                    break;
                case AIState.PreparingAttack:
                    Debug.Log("Exiting PreparingAttack State");
                    break;
                case AIState.Attacking:
                    Debug.Log("Exiting Attacking State");
                    break;
            }
        }

        private void PatrolUpdate()
        {
            if (enemyDetection.PlayerFound)
            {
                ChangeState(AIState.Chasing);
            }
            if (health.CurrentHealth / health.MaxHealth < fleeHealthThreshold)
            {
                ChangeState(AIState.Fleeing);
            }
        }

        private void ChaseUpdate()
        {
            if (!enemyDetection.PlayerFound)
            {
                ChangeState(AIState.Patrolling);
            }
            if (health.CurrentHealth / health.MaxHealth < fleeHealthThreshold)
            {
                ChangeState(AIState.Fleeing);
            }
            if (Vector2.Distance(transform.position, enemyDetection.PlayerPosition) <= 2f)
            {
                ChangeState(AIState.PreparingAttack);
            }
        }

        private void FleeUpdate()
        {
            fleeTimer -= Time.deltaTime;
            if (fleeTimer <= 0)
            {
                ChangeState(AIState.Patrolling);
            }
            else
            {
                Flee();
            }
        }

        private void Flee()
        {
            Vector2 fleeDirection = (transform.position - enemyDetection.PlayerPosition).normalized;
            fleeDirection += new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
            enemyMovement.ForceJump(fleeDirection, 0.5f);
        }

        private void PrepareAttackUpdate()
        {
            if (!enemyDetection.PlayerFound)
            {
                ChangeState(AIState.Patrolling);
                return;
            }

            float distanceToPlayer = Vector2.Distance(transform.position, enemyDetection.PlayerPosition);

            if (distanceToPlayer <= closeAttackRange)
            {
                combatSystem.SetCurrentAttackStrategy(combatSystem.BasicAttackStrategy);
                ChangeState(AIState.Attacking);
            }
            else if (distanceToPlayer <= midAttackRange)
            {
                combatSystem.SetCurrentAttackStrategy(combatSystem.SpecialAttackStrategy);
                ChangeState(AIState.Attacking);
            }
            else
            {
                ChangeState(AIState.Chasing);
            }
        }

        private void AttackUpdate()
        {
            // Check for conditions to switch to other states
            if (!enemyDetection.PlayerFound)
            {
                ChangeState(AIState.Patrolling);
            }
            // Check if the enemy is too far to attack
            if (Vector2.Distance(transform.position, enemyDetection.PlayerPosition) > 2f)
            {
                ChangeState(AIState.Chasing);
            }
            // Perform attack
            combatSystem.PerformAttack();
        }

        private void HandleDeath()
        {
            // Obtener el componente PlayerStats del jugador
            GameObject player = GameObject.FindGameObjectWithTag("Player"); // Asegúrate de que tu jugador tenga el tag "Player"
            if (player != null)
            {
                PlayerStats playerStats = player.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    // Añadir la experiencia al jugador usando el valor del Scriptable Object
                    playerStats.AddExperience((int)experienceConfig.ExperienceReward);
                }
                else
                {
                    Debug.LogError("PlayerStats component not found on the player.");
                }
            }
            else
            {
                Debug.LogError("Player not found with tag 'Player'.");
            }
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            health.OnDeath -= HandleDeath;
        }

        private void HandleDeath()
        {
            // Obtener el componente PlayerStats del jugador
            GameObject player = GameObject.FindGameObjectWithTag("Player"); // Asegúrate de que tu jugador tenga el tag "Player"
            if (player != null)
            {
                PlayerStats playerStats = player.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    // Añadir la experiencia al jugador usando el valor del Scriptable Object
                    playerStats.AddExperience((int)experienceConfig.ExperienceReward);
                }
                else
                {
                    Debug.LogError("PlayerStats component not found on the player.");
                }
            }
            else
            {
                Debug.LogError("Player not found with tag 'Player'.");
            }
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            health.OnDeath -= HandleDeath;
        }
    }
}
