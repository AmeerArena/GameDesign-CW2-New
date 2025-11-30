using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public string previousScene;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        previousScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("TiledScene");
    }

    public void LoadSettings()
    {
        previousScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("SettingsScene");
    }

    public void LoadGameOver()
    {
        Time.timeScale = 0f;
        previousScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("GameOverScene");
    }

    public void BackToPreviousScene()
    {
        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(previousScene))
            SceneManager.LoadScene(previousScene);
        else SceneManager.LoadScene("MainMenuScene");
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
