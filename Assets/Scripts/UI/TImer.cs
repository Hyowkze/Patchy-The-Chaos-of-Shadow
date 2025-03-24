using UnityEngine;
using TMPro;
using Core.Managers;

public class Timer : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private float startTime = 300f; // Tiempo inicial en segundos (5 minutos)
    private float currentTime;
    private bool isRunning = true;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI timerText;

    private void Start()
    {
        currentTime = startTime;
        UpdateTimerUI();
    }

    private void Update()
    {
        if (isRunning)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
            {
                currentTime = 0;
                isRunning = false;
                TimerEnded();
            }
            UpdateTimerUI();
        }
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = $"{minutes:D2}:{seconds:D2}"; // Formato MM:SS
    }

    private void TimerEnded()
    {
        Debug.Log("¡Tiempo agotado!");
        GameManager.Instance.GameOver();
    }

    public void AddTime(float extraTime)
    {
        currentTime += extraTime;
        UpdateTimerUI();
    }
}
