using UnityEngine;
using System;

namespace Core.Events
{
    public static class GameEvents
    {
        // Player Events
        public static event Action<float> OnPlayerHealthChanged;
        public static event Action<int> OnExperienceGained;
        public static event Action OnPlayerDeath;

        // Game State Events
        public static event Action OnGamePaused;
        public static event Action OnGameResumed;
        public static event Action OnGameOver;

        // Combat Events
        public static event Action<float> OnDamageDealt;
        public static event Action<Vector2> OnAttackPerformed;

        // Generic Trigger Method
        public static void TriggerEvent<T>(Action<T> gameEvent, T value, string eventName)
        {
            gameEvent?.Invoke(value);
            Debug.Log($"{eventName} Event Triggered with value: {value}");
        }

        public static void TriggerEvent(Action gameEvent, string eventName)
        {
            gameEvent?.Invoke();
            Debug.Log($"{eventName} Event Triggered");
        }

        // Specific Trigger Methods (Optional - for convenience)
        public static void TriggerPlayerHealthChanged(float newHealth) => TriggerEvent(OnPlayerHealthChanged, newHealth, "Player Health Changed");
        public static void TriggerExperienceGained(int amount) => TriggerEvent(OnExperienceGained, amount, "Experience Gained");
        public static void TriggerPlayerDeath() => TriggerEvent(OnPlayerDeath, "Player Death");
        public static void TriggerGamePaused() => TriggerEvent(OnGamePaused, "Game Paused");
        public static void TriggerGameResumed() => TriggerEvent(OnGameResumed, "Game Resumed");
        public static void TriggerGameOver() => TriggerEvent(OnGameOver, "Game Over");
        public static void TriggerDamageDealt(float amount) => TriggerEvent(OnDamageDealt, amount, "Damage Dealt");
        public static void TriggerAttackPerformed(Vector2 direction) => TriggerEvent(OnAttackPerformed, direction, "Attack Performed");
    }
}
