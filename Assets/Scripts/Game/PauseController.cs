using UnityEngine;

public class PauseController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pausePanel;

    [Header("Input")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

    public static bool IsGamePaused { get; private set; } = false;

    // when true ignore ESC key
    public static bool EscapeBlocked { get; set; } = false;

    public static PauseController Instance { get; private set; }

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        Time.timeScale = 1f;
        IsGamePaused = false;
        EscapeBlocked = false;
    }

    private void Update()
    {
        
        if (EscapeBlocked)
            return;

        if (Input.GetKeyDown(pauseKey))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        SetPause(!IsGamePaused, true);
    }

    public void ResumeGame()
    {
        SetPause(false, true);
    }

    public void QuitToMainMenu()
    {
        SetPause(false, true);
        GameManager.Instance.LoadMainMenu();
    }

    
    public static void SetPause(bool pause, bool showPauseMenu = true)
    {
        if (Instance == null)
        {
            Debug.LogWarning("PauseController.SetPause called but no instance exists.");
            IsGamePaused = pause;
            Time.timeScale = pause ? 0f : 1f;
            return;
        }

        Instance.SetPauseInstance(pause, showPauseMenu);
    }

    private void SetPauseInstance(bool pause, bool showPauseMenu)
    {
        IsGamePaused = pause;
        Time.timeScale = pause ? 0f : 1f;

        if (pausePanel == null)
            return;

        if (showPauseMenu)
        {
            pausePanel.SetActive(pause);
        }
        
    }
}
