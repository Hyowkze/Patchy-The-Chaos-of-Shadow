using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Core.Managers;
using Core.Player; // <--- Added this using directive

namespace Core.Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private PlayerStats playerStats;
        private readonly HashSet<string> defeatedEnemyIds = new HashSet<string>();
        private bool isGameOver;

        public event System.Action OnGameOver;
        
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
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else
            {
                Destroy(gameObject);
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

        public void TriggerGameOver(float delay = 2f)
        {
            if (isGameOver) return;
            
            isGameOver = true;
            OnGameOver?.Invoke();
            StartCoroutine(HandleGameOver(delay));
        }

        private IEnumerator HandleGameOver(float delay)
        {
            yield return new WaitForSeconds(delay);
            // Aquí puedes cargar la escena inicial o reiniciar el nivel
            SceneManager.LoadScene(
                SceneManager.GetActiveScene().buildIndex
            );
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                applicationIsQuitting = true;
            }
        }
    }
}
