using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject PanelPause;
    public Button BtnResume, BtnMainMenu, BtnQuit;

    private bool isPaused = false;

    void Start()
    {
        BtnResume.onClick.AddListener(Resume);
        BtnMainMenu.onClick.AddListener(GoToMainMenu);
        BtnQuit.onClick.AddListener(QuitGame);

        PanelPause.SetActive(false); // Asegurarse de que el menú está oculto al inicio
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        PanelPause.SetActive(false);
        Time.timeScale = 1f; 
        isPaused = false;
    }

    public void Pause() 
    {
        PanelPause.SetActive(true);
        Time.timeScale = 0f; 
        isPaused = true;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // Restablecer el tiempo antes de cambiar de escena
        SceneManager.LoadScene("MenuPrincipal");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Juego cerrado"); 
    }
}
