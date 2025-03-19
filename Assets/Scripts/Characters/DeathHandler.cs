using UnityEngine;
using Core.Characters;
using Core.Enemy.AI;
using Core.Managers;
using Core.Player; // <--- Added this using directive

[RequireComponent(typeof(Health))]
public class DeathHandler : MonoBehaviour
{
    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        health.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        health.OnDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        if (TryGetComponent<EnemyAIStateMachine>(out var enemy))
        {
            GameManager.Instance.RegisterDefeatedEnemy(gameObject.name);
            Destroy(gameObject);
        }
        else if (TryGetComponent<PlayerStats>(out var player))
        {
            GameManager.Instance.HandlePlayerDeath();
            gameObject.SetActive(false);
        }
    }
}
