using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Core.Player; 
using System;

namespace Core.Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private PlayerStats playerStats;
        private readonly HashSet<string> defeatedEnemyIds = new HashSet<string>();
        private bool isGameOver;

        public event Action OnGameOver; // Use System.Action

        // Thread-safe singleton
        private static readonly object _lock = new object();
        private static GameManager instance;

        public static GameManager Instance
        {
            get
            {
                if (applicationIsQuitting)
                {
                    return null;
                }

                lock (_lock)
                {
                    if (instance == null)
                    {
                        instance = FindAnyObjectByType<GameManager>();
                        if (instance == null)
                        {
                            var singleton = new GameObject("GameManager");
                            instance = singleton.AddComponent<GameManager>();
                            DontDestroyOnLoad(singleton);
                        }
                    }
                    return instance;
                }
            }
        }

        private static bool applicationIsQuitting = false;

        private void Awake()
        {
            lock (_lock)
            {
                if (instance != null && instance != this)
                {
                    Destroy(gameObject);
                    return;
                }

                instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
        }

        private void Initialize()
        {
            if (playerStats == null)
            {
                playerStats = FindAnyObjectByType<PlayerStats>();
            }
        }

        public void RegisterDefeatedEnemy(string enemyId)
        {
            defeatedEnemyIds.Add(enemyId);
        }

        public bool IsEnemyDefeated(string enemyId)
        {
            return defeatedEnemyIds.Contains(enemyId);
        }

        public void HandlePlayerDeath()
        {
            playerStats.HandleDeath();
        }

        public void ResetGame()
        {
            defeatedEnemyIds.Clear();
        }

        public void GameOver() // Changed from TriggerGameOver
        {
            if (isGameOver) return;

            isGameOver = true;
            OnGameOver?.Invoke();
            StartCoroutine(HandleGameOver()); // Removed delay parameter
        }

        private IEnumerator HandleGameOver() // Removed delay parameter
        {
            yield return new WaitForSeconds(2f); // Fixed delay
            // Aquí puedes cargar la escena inicial o reiniciar el nivel
            SceneManager.LoadScene(
                SceneManager.GetActiveScene().buildIndex
            );
            isGameOver = false; // Reset isGameOver flag
        }

        private void OnDestroy()
        {
            lock (_lock)
            {
                if (instance == this)
                {
                    applicationIsQuitting = true;
                    instance = null; // Clear the instance
                }
            }
        }
    }
}
