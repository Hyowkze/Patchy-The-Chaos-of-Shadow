using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Core.Events;
using Core.Player;
using Core.Characters;
using Player.Movement;
using Core.Managers; // <--- Added this using directive

namespace Core.UI
{
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
        
        [Header("Game Over UI")]
        [SerializeField] private GameObject gameOverPanel;

        [Header("References")]
        [SerializeField] private PlayerStats playerReference;
        [SerializeField] private Health playerHealth;
        [SerializeField] private PatchyMovement playerMovement;

        private void Awake()
        {
            ValidateReferences();
        }

        private void ValidateReferences()
        {
            if (playerReference == null || playerHealth == null || playerMovement == null)
            {
                Debug.LogError($"Missing required references on {gameObject.name}");
                enabled = false;
                return;
            }
        }

        private void Start()
        {
            SubscribeToEvents();
            InitializeUI();
        }

        private void SubscribeToEvents()
        {
            playerHealth.OnHealthValueChanged += UpdateHealthUI;            
            playerReference.OnExperienceChanged += UpdateExperienceUI;

            if (GameManager.Instance != null) 
            {
                GameManager.Instance.OnGameOver += ShowGameOver;
            }
        }

        private void InitializeUI()
        {
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
            if (playerHealth != null)
            {
                playerHealth.OnHealthValueChanged -= UpdateHealthUI;
            }
            if (playerMovement != null)
            {                
            }
            if (playerReference != null)
            {
                playerReference.OnExperienceChanged -= UpdateExperienceUI;
            }
        }
    }
}
