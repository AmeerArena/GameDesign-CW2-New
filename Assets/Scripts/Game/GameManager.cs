using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Scene Names")]
    [Tooltip("Name of your main menu scene.")]
    public string mainMenuSceneName = "MainMenu";

    [Tooltip("Name of the first gameplay scene (e.g. Level1, GameScene, etc.).")]
    public string firstGameSceneName = "GameScene";

    private string _currentSceneName;

    private void Awake()
    {
        // Singleton pattern – only one GameManager across all scenes
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _currentSceneName = SceneManager.GetActiveScene().name;

        // Just in case we ever end up paused on load
        SafeUnpause();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _currentSceneName = scene.name;

        // Make sure game isn't stuck paused after scene changes
        SafeUnpause();
    }

    // =========================
    //  PUBLIC API – BUTTON HOOKS
    // =========================

    /// <summary>
    /// Load the main menu scene (for menu buttons).
    /// </summary>
    public void LoadMainMenu()
    {
        StartCoroutine(LoadSceneRoutine(mainMenuSceneName));
    }

    /// <summary>
    /// Start a new game from the first gameplay scene.
    /// </summary>
    public void StartNewGame()
    {
        StartCoroutine(LoadSceneRoutine(firstGameSceneName));
    }

    /// <summary>
    /// Generic scene loader you can call from other scripts.
    /// </summary>
    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("GameManager.LoadScene called with empty sceneName.");
            return;
        }

        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    /// <summary>
    /// Quit the game (works in build, stops play mode in editor).
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // =========================
    //  INTERNAL HELPERS
    // =========================

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        // Ensure game is unpaused and ESC is free before changing scenes
        SafeUnpause();

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        if (loadOp == null)
        {
            Debug.LogError($"Failed to load scene '{sceneName}'. Check that it's added to Build Settings.");
            yield break;
        }

        while (!loadOp.isDone)
        {
            yield return null;
        }

        // Just to be extra safe
        SafeUnpause();
    }

    /// <summary>
    /// Centralised unpause logic so we don't get stuck with Time.timeScale = 0
    /// or EscapeBlocked staying true across scenes.
    /// </summary>
    private void SafeUnpause()
    {
        Time.timeScale = 1f;

        // These assume your PauseController exists as we've been using it in NPC.
        // If not, you can safely remove these calls.
        try
        {
            // If your SetPause signature is (bool paused, bool showPauseMenu = true)
            PauseController.SetPause(false, false);
            PauseController.EscapeBlocked = false;
        }
        catch
        {
            // If PauseController isn't present in some test scenes, don't hard-crash.
        }
    }
}
