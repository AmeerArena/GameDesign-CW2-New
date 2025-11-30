using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public void OnPlayPressed()
    {
        GameManager.Instance.StartGame();
    }

    public void OnSettingsPressed()
    {
        GameManager.Instance.LoadSettings();
    }

    public void OnQuitPressed()
    {
        GameManager.Instance.QuitGame();
    }
}
