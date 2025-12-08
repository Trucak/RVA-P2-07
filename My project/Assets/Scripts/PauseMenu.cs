using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public bool isPaused;

    void Start()
    {
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (pauseMenu.activeSelf)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f; // Congela o jogo
        AudioListener.pause = true; // Pausa todos os áudios
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f; // Retoma o jogo
        AudioListener.pause = false; // Retoma todos os áudios
        isPaused = false;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // Assegura que o tempo está normalizado ao sair
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        // Esta parte faz o jogo fechar mesmo no editor:
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
