using UnityEngine;
using System;
using Core.Characters;

namespace Core.Characters
{
    public class Health : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth;

        public float MaxHealth => maxHealth;
        public float CurrentHealth => currentHealth;

        public event Action<float, float> OnHealthValueChanged;
        public event Action OnDeath;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        private void Start()
        {
            ValidateHealth();
        }

        private void ValidateHealth()
        {
            if (maxHealth <= 0)
            {
                Debug.LogError($"Invalid max health value on {gameObject.name}");
                maxHealth = 100f;
            }
            currentHealth = maxHealth;
        }

        public void TakeDamage(float damage)
        {
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            OnHealthValueChanged?.Invoke(currentHealth, maxHealth);

            if (currentHealth <= 0)
            {
                OnDeath?.Invoke();
            }
        }

        public void Heal(float amount)
        {
            currentHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            OnHealthValueChanged?.Invoke(currentHealth, maxHealth);
        }

        public void SetMaxHealth(float value)
        {
            maxHealth = Mathf.Max(1, value);
            currentHealth = Mathf.Min(currentHealth, maxHealth);
            OnHealthValueChanged?.Invoke(currentHealth, maxHealth);
        }
    }
}
