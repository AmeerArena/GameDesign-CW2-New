using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Scene Names")]
    [SerializeField] private string mainMenuSceneName = "MainMenuScene";
    [SerializeField] private string settingsSceneName = "SettingsScene";
    [SerializeField] private string gameSceneName = "TiledScene";

    [Header("Navigation")]
    public string previousScene;

    [Header("Day System")]
    public DayCounter dayCounter;
    public int currentDay = 1;

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

    private void Start()
    {
        Time.timeScale = 1f;
    }

    public void StartGame()
    {
        previousScene = SceneManager.GetActiveScene().name;
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    public void LoadSettings()
    {
        previousScene = SceneManager.GetActiveScene().name;
        Time.timeScale = 1f;
        SceneManager.LoadScene(settingsSceneName);
    }

    public void BackToPreviousScene()
    {
        Time.timeScale = 1f;

        if (!string.IsNullOrEmpty(previousScene))
        {
            string target = previousScene;
            previousScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(target);
        }
        else
        {
            LoadMainMenu();
        }
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        previousScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void SetDay(int day)
    {
        currentDay = Mathf.Max(1, day);

        if (dayCounter != null)
        {
            dayCounter.UpdateDayText(currentDay);
        }
    }

    public void IncrementDay()
    {
        currentDay++;

        if (dayCounter != null)
        {
            dayCounter.UpdateDayText(currentDay);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #endif
    }

    private void Update()
{
    if (Input.GetKeyDown(KeyCode.Equals))   // press = key
    {
        IncrementDay();
    }
}
}
