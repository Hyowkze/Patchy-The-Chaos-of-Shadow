using UnityEngine;

namespace Core.Events
{
    public static class GameEvents
    {
        // Player Events
        public static event System.Action<float> OnPlayerHealthChanged;
        public static event System.Action<int> OnExperienceGained;
        public static event System.Action OnPlayerDeath;
        
        // Game State Events
        public static event System.Action OnGamePaused;
        public static event System.Action OnGameResumed;
        public static event System.Action OnGameOver;
        
        // Combat Events
        public static event System.Action<float> OnDamageDealt;
        public static event System.Action<Vector2> OnAttackPerformed;

        public static void TriggerPlayerHealthChanged(float newHealth) 
            => OnPlayerHealthChanged?.Invoke(newHealth);

        public static void TriggerExperienceGained(int amount) 
            => OnExperienceGained?.Invoke(amount);

        public static void TriggerPlayerDeath() 
            => OnPlayerDeath?.Invoke();

        public static void TriggerGamePaused() 
            => OnGamePaused?.Invoke();

        public static void TriggerGameResumed() 
            => OnGameResumed?.Invoke();

        public static void TriggerGameOver() 
            => OnGameOver?.Invoke();

        public static void TriggerDamageDealt(float amount) 
            => OnDamageDealt?.Invoke(amount);

        public static void TriggerAttackPerformed(Vector2 direction) 
            => OnAttackPerformed?.Invoke(direction);
    }
}
