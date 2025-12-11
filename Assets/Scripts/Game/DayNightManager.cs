using UnityEngine;
using UnityEngine.SceneManagement;

public class DayNightManager : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string daySceneName = "TiledScene";
    [SerializeField] private string nightSceneName = "NightScene";

    // Called by the house during the day
    public void GoToNight()
    {
        // NPC or quest processing here
        SceneManager.LoadScene(nightSceneName);
    }

    // Called by the "Next Day" button in the night scene
    public void GoToNextDay()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.IncrementDay();
        }

        SceneManager.LoadScene(daySceneName);
    }
}
