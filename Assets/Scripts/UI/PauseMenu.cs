using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Button Panel")]
    public GameObject panel;

    public void PausePanelButton()
    {
        Time.timeScale = 0;
        panel.SetActive(true);
    }

    public void ResumeButton() 
    {
        Time.timeScale = 1;
        panel.SetActive(false);
    }

    public void GotoMainMenuButton() 
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1.0f;
    }
    public void QuitButton() 
    {
        Application.Quit();
    }
}
