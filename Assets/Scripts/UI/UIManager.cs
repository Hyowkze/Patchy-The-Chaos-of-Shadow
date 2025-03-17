using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Health UI")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Sprint UI")]
    [SerializeField] private Slider sprintSlider;

    [Header("Dash UI")]
    [SerializeField] private Image dashCooldownImage;

    [Header("Experience UI")]
    [SerializeField] private TextMeshProUGUI experienceText;
    // Removemos el slider ya que no necesitamos mostrar progreso

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;

    private void Start()
    {
        // Get references
        var player = GameObject.FindGameObjectWithTag("Player");
        var health = player.GetComponent<Health>();
        var movement = player.GetComponent<PatchyMovement>();
        var stats = player.GetComponent<PlayerStats>();

        // Subscribe to events
        if (health != null)
        {
            health.OnHealthValueChanged += UpdateHealthUI;
        }

        if (movement != null)
        {
            movement.OnDashCooldownUpdate += UpdateDashUI;
            movement.OnSprintValueChanged += UpdateSprintUI;
        }

        if (stats != null)
        {
            stats.OnExperienceChanged += UpdateExperienceUI;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver += ShowGameOver;
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    private void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        if (healthSlider != null)
            healthSlider.value = currentHealth / maxHealth;
        
        if (healthText != null)
            healthText.text = $"{Mathf.Ceil(currentHealth)}/{maxHealth}";
    }

    private void UpdateSprintUI(float sprintValue)
    {
        if (sprintSlider != null)
            sprintSlider.value = sprintValue;
    }

    private void UpdateDashUI(float cooldownPercentage)
    {
        if (dashCooldownImage != null)
            dashCooldownImage.fillAmount = 1 - cooldownPercentage;
    }

    private void UpdateExperienceUI(int experience)
    {
        if (experienceText != null)
            experienceText.text = $"EXP: {experience}";
    }

    private void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver -= ShowGameOver;
        }
    }
}
