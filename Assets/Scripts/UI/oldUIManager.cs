using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject startMenu;
    public GameObject pauseMenu;
    public GameObject endMenu;
    public GameObject settingsMenu;

    void Start()
    {
        startMenu.SetActive(true);
        pauseMenu.SetActive(false);
        endMenu.SetActive(false);
        settingsMenu.SetActive(false);
    }

    public void PlayGame()
    {
        startMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OpenSettings()
    {
        settingsMenu.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsMenu.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit pressed");
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

}
