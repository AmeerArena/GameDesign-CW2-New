using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    [Header("Settings UI")]
    [SerializeField] private DisplayResolution displayResolution;
    [SerializeField] private DisplayMode displayMode;

    void Start()
    {
        LoadSettings();
    }

    void LoadSettings()
    {
        int resIndex = PlayerPrefs.GetInt("ResolutionIndex", 0);
        int modeInt = PlayerPrefs.GetInt(
            "DisplayMode",
            (int)FullScreenMode.FullScreenWindow
        );

        displayResolution.SetFromSaved(resIndex);
        displayMode.SetFromSaved((FullScreenMode)modeInt);
    }

    public void OnApplyPressed()
    {
        ApplySettings();
        SaveSettings();
    }

    void ApplySettings()
    {
        Resolution res = displayResolution.GetSelectedResolution();
        Screen.SetResolution(
            res.width,
            res.height,
            displayMode.SelectedMode
        );
    }

    void SaveSettings()
    {
        PlayerPrefs.SetInt(
            "ResolutionIndex",
            displayResolution.SelectedResolutionIndex
        );

        PlayerPrefs.SetInt(
            "DisplayMode",
            (int)displayMode.SelectedMode
        );

        PlayerPrefs.Save();
    }

    public void OnBackPressed()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.BackToPreviousScene();
        else
            Debug.LogError("GameManager missing in SettingsScene");
    }
}
