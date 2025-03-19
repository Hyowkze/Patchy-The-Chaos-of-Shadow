using UnityEngine;
using Core.Characters; // <--- Added this using directive

[RequireComponent(typeof(Health))]
public class HealthRegeneration : MonoBehaviour
{
    [SerializeField] private float regenerationRate = 1f;
    [SerializeField] private float regenerationDelay = 3f;
    
    private float regenerationTimer;
    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
        regenerationTimer = 0f;
    }

    private void OnEnable()
    {
        health.OnHealthValueChanged += StartRegeneration;
    }

    private void OnDisable()
    {
        health.OnHealthValueChanged -= StartRegeneration;
    }

    private void Update()
    {
        if (regenerationTimer > 0)
        {
            regenerationTimer -= Time.deltaTime;
            if (regenerationTimer <= 0)
            {
                RegenerateHealth();
            }
        }
    }

    private void RegenerateHealth()
    {
        if (health.CurrentHealth >= health.MaxHealth) return;

        health.Heal(regenerationRate * Time.deltaTime);
        
        if (health.CurrentHealth < health.MaxHealth)
        {
            StartRegeneration(health.CurrentHealth, health.MaxHealth);
        }
    }

    private void StartRegeneration(float current, float max)
    {
        regenerationTimer = regenerationDelay;
    }
}
