using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Core.Events;
using Core.Player;
using Core.Characters;
using Player.Movement;
using Core.Managers;

namespace Core.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Health UI")]
        [SerializeField] private Slider healthSlider;
        // Removemos la referencia al texto de salud

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
            if (playerReference == null)
            {
                playerReference = FindAnyObjectByType<PlayerStats>();
                Debug.Log($"Attempting to find PlayerStats: {(playerReference != null ? "Found" : "Not Found")}");
            }

            if (playerReference != null)
            {
                if (playerHealth == null)
                {
                    playerHealth = playerReference.GetComponent<Health>();
                    Debug.Log($"Attempting to get Health component: {(playerHealth != null ? "Found" : "Not Found")}");
                }

                if (playerMovement == null)
                {
                    playerMovement = playerReference.GetComponent<PatchyMovement>();
                    Debug.Log($"Attempting to get PatchyMovement component: {(playerMovement != null ? "Found" : "Not Found")}");
                }
            }

            // Validar UI elements
            if (healthSlider == null)
                Debug.LogWarning("Health Slider is not assigned in UIManager");

            if (sprintSlider == null)
                Debug.LogWarning("Sprint Slider is not assigned in UIManager");

            if (dashCooldownImage == null)
                Debug.LogWarning("Dash Cooldown Image is not assigned in UIManager");

            if (experienceText == null)
                Debug.LogWarning("Experience Text is not assigned in UIManager");

            if (gameOverPanel == null)
                Debug.LogWarning("Game Over Panel is not assigned in UIManager");
        }

        private void Start()
        {
            ValidateReferences();
            if (enabled) // Solo si las validaciones pasaron
            {
                SubscribeToEvents();
                InitializeUI();
            }
        }

        private void SubscribeToEvents()
        {
            if (playerHealth != null)
                playerHealth.OnHealthValueChanged += UpdateHealthUI;
                
            if (playerReference != null)
                playerReference.OnExperienceChanged += UpdateExperienceUI;
                
            if (GameManager.Instance != null)
                GameManager.Instance.OnGameOver += ShowGameOver;
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
                GameManager.Instance.OnGameOver -= ShowGameOver;
                
            if (playerHealth != null)
                playerHealth.OnHealthValueChanged -= UpdateHealthUI;
                
            if (playerReference != null)
                playerReference.OnExperienceChanged -= UpdateExperienceUI;
        }
    }
}
