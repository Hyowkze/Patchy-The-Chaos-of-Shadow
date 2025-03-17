using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    [Header("Experience Settings")]
    [SerializeField] private int currentExperience = 0;
    [SerializeField] private int experiencePenalty = 50;
    [SerializeField] private float experiencePenaltyPercentage = 0.1f;

    [Header("Respawn Settings")]
    [SerializeField] private float respawnDelay = 2f;

    [Header("Game Over Settings")]
    [SerializeField] private float gameOverDelay = 1f; // Now used
    private bool isGameOver = false;

    private Vector3 lastCheckpoint;
    private Health healthComponent;

    public event System.Action<int> OnExperienceChanged;
    public int CurrentExperience => currentExperience;

    private void Awake()
    {
        healthComponent = GetComponent<Health>();
        lastCheckpoint = transform.position;
    }

    public void SetCheckpoint(Vector3 position)
    {
        lastCheckpoint = position;
    }

    public void HandleDeath()
    {
        // Calculate experience loss
        int experienceLoss = Mathf.Min(
            experiencePenalty,
            Mathf.RoundToInt(currentExperience * experiencePenaltyPercentage)
        );
        
        int newExperience = currentExperience - experienceLoss;
        
        if (newExperience <= 0)
        {
            GameOver();
            return;
        }

        currentExperience = newExperience;
        OnExperienceChanged?.Invoke(currentExperience);
        
        // Random respawn
        Invoke(nameof(RandomRespawn), respawnDelay);
    }

    private void GameOver()
    {
        isGameOver = true;
        currentExperience = 0;
        OnExperienceChanged?.Invoke(currentExperience);
        GameManager.Instance.TriggerGameOver(gameOverDelay); // Pass the delay
    }

    private void RandomRespawn()
    {
        if (isGameOver) return;
        
        Vector3 randomPosition = GridManager.Instance.GetRandomValidPosition();
        transform.position = randomPosition;
        gameObject.SetActive(true);
        healthComponent.RestartRegeneration();
    }

    public void AddExperience(int amount)
    {
        currentExperience += amount;
        OnExperienceChanged?.Invoke(currentExperience);
    }
}