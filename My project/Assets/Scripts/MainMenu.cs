using UnityEngine;
using UnityEngine.SceneManagement; // Não te esqueças disto

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Garden");
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