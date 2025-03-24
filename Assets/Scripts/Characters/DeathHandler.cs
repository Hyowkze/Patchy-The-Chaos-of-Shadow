using UnityEngine;
<<<<<<< Updated upstream
using Core.Characters;
using Core.Enemy.AI;
using Core.Managers;
using Core.Player; // <--- Added this using directive
=======
using Core.Player;
using Core.Utils;
using Core.Managers;
>>>>>>> Stashed changes

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
<<<<<<< Updated upstream
            GameManager.Instance.HandlePlayerDeath();
            gameObject.SetActive(false);
=======
            playerStats.OnExperienceChanged += HandleExperienceChanged;
        }

        protected override void UnsubscribeFromEvents()
        {
            if (playerStats != null)
            {
                playerStats.OnExperienceChanged -= HandleExperienceChanged;
            }
        }

        private void HandleExperienceChanged(int experience)
        {
            if (experience <= 0)
            {
                playerStats.HandleDeath();
                GameManager.Instance.GameOver(); // Call GameOver directly
            }
>>>>>>> Stashed changes
        }
    }
}
