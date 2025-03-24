using UnityEngine;
using System.Collections;
using Core.Player;
using Core.Managers;
using Core.Characters;

namespace Core.Player
{
    [RequireComponent(typeof(Health))]
    public class PlayerStats : MonoBehaviour
    {
        [Header("Experience Settings")]
        [SerializeField] private int currentExperience = 0;
        [SerializeField] private int experiencePenalty = 50;
        [SerializeField] private float experiencePenaltyPercentage = 0.1f;
        [SerializeField] private int minExperienceForRespawn = 1; // Minimum experience required for respawn

        [Header("Invulnerability Settings")]
        [SerializeField] private float invulnerabilityDuration = 2f;

        private bool isGameOver = false;

        private Vector3 lastCheckpoint;
        private Health healthComponent;

        public event System.Action<int> OnExperienceChanged;
        public int CurrentExperience => currentExperience;

        private void Awake()
        {
            InitializeComponents();
            InitializeStartingValues();
        }

        private void InitializeComponents()
        {
            healthComponent = GetComponent<Health>();
            if (healthComponent == null)
            {
                Debug.LogError($"Missing Health component on {gameObject.name}");
                enabled = false;
            }
        }

        private void InitializeStartingValues()
        {
            lastCheckpoint = transform.position;
            currentExperience = 0;
            isGameOver = false;
        }

        public void SetCheckpoint(Vector3 position)
        {
            lastCheckpoint = position;
        }

        public void HandleDeath()
        {
            if (isGameOver) return;

            int experienceLoss = CalculateExperienceLoss();
            UpdateExperience(experienceLoss);

            if (currentExperience < minExperienceForRespawn)
            {
                GameOver();
                return;
            }

            Respawn();
        }

        private int CalculateExperienceLoss()
        {
            return Mathf.Min(
                experiencePenalty,
                Mathf.RoundToInt(currentExperience * experiencePenaltyPercentage)
            );
        }

        private void UpdateExperience(int loss)
        {
            currentExperience = Mathf.Max(0, currentExperience - loss);
            OnExperienceChanged?.Invoke(currentExperience);
        }

        private void GameOver()
        {
            isGameOver = true;
            currentExperience = 0;
            OnExperienceChanged?.Invoke(currentExperience);
        }

        private void Respawn()
        {
            if (isGameOver) return;

            //Respawn in the same position
            transform.position = transform.position;
            gameObject.SetActive(true);
            StartCoroutine(InvulnerabilityRoutine()); // Start invulnerability
        }

        private IEnumerator InvulnerabilityRoutine()
        {
            healthComponent.SetInvulnerable(true); // Set invulnerability in Health component
            yield return new WaitForSeconds(invulnerabilityDuration);
            healthComponent.SetInvulnerable(false); // Remove invulnerability in Health component
        }

        public void AddExperience(int amount)
        {
            currentExperience += amount;
            OnExperienceChanged?.Invoke(currentExperience);
        }
    }
}
