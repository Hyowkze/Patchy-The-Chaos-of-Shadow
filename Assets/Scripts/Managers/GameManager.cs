using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<GameManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    instance = go.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }

    [SerializeField] private PlayerStats playerStats;
    
    private readonly HashSet<GameObject> defeatedEnemies = new HashSet<GameObject>();
    private bool isGameOver;

    public event System.Action OnGameOver;

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
            playerStats = FindFirstObjectByType<PlayerStats>();
        }
    }

    public void RegisterDefeatedEnemy(GameObject enemy)
    {
        defeatedEnemies.Add(enemy);
    }

    public bool IsEnemyDefeated(GameObject enemy)
    {
        return defeatedEnemies.Contains(enemy);
    }

    public void HandlePlayerDeath()
    {
        playerStats.HandleDeath();
    }

    public void ResetGame()
    {
        defeatedEnemies.Clear();
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
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
