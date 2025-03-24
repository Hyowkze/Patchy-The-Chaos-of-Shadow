using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Mision1"); // Carga la escena de juego
    }

    public void ShowCredits()
    {
        SceneManager.LoadScene("Credits"); // Carga la escena de créditos
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Juego cerrado"); 
    }
}

