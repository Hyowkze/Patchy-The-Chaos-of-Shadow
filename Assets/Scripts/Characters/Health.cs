using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth; // No need to be serialized

    [Header("Regeneration Settings")]
    [SerializeField] private bool canRegenerate = false;
    [SerializeField] private float regenerationRate = 1f; // Amount to regenerate per second
    [SerializeField] private float regenerationDelay = 2f; // Time to wait before starting regeneration

    [Header("Events")]
    public UnityEvent OnHealthChanged; // Called when health changes
    public UnityEvent OnDeath; // Called when health reaches 0

    private float timeSinceLastDamage;
    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    private Coroutine regenerationCoroutine; //Reference to the current coroutine.

    private void Awake()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke();
    }

    public void TakeDamage(float damageAmount)
    {
        // Ensure that damage cannot cause health to go below 0
        currentHealth -= damageAmount;
         currentHealth = Mathf.Max(currentHealth, 0f);

        timeSinceLastDamage = Time.time;
        OnHealthChanged?.Invoke();

        if (currentHealth <= 0f)
        {
            Die();
        }

        // Only restart regeneration if canRegenerate is true
        if (canRegenerate)
        {
            RestartRegeneration();
        }
    }

    public void StartRegeneration()
    {
        // Check if already regenerating to avoid starting multiple coroutines
        if (canRegenerate && regenerationCoroutine == null)
        {
            regenerationCoroutine = StartCoroutine(RegenerateHealthCoroutine());
        }
    }

    public void StopRegeneration()
    {
        if (regenerationCoroutine != null)
        {
            StopCoroutine(regenerationCoroutine);
            regenerationCoroutine = null;
        }
    }

    private IEnumerator RegenerateHealthCoroutine()
    {
        // Wait for the delay before starting regeneration
        yield return new WaitForSeconds(regenerationDelay);

        // Continuously regenerate health until max health is reached
        while (currentHealth < maxHealth)
        {
            // Regenerate health
            currentHealth += regenerationRate * Time.deltaTime;
            currentHealth = Mathf.Min(currentHealth, maxHealth); //Ensure that the health cant be greater than the maxhealth
            OnHealthChanged?.Invoke();
            yield return null; // Wait for the next frame
        }
        // Stop the coroutine when max health is reached
        StopRegeneration();
    }

    private void Die()
    {
        OnDeath?.Invoke();
        Destroy(gameObject); //You can change this to a virtual method.
    }

    public void RestartRegeneration()
    {
        // Stop any existing regeneration
        StopRegeneration();
        // Start regeneration after delay
        StartRegeneration();
    }
}
