using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    [Header("Settings UI")]
    public DisplayResolution displayResolution;
    public DisplayMode displayMode;

    void Start()
    {
        LoadSettings();
    }

    void LoadSettings()
    {
        int resIndex = PlayerPrefs.GetInt("ResolutionIndex", 0);
        int mode = PlayerPrefs.GetInt("DisplayMode", (int)FullScreenMode.FullScreenWindow);

        displayResolution.SetFromSaved(resIndex);
        displayMode.SetFromSaved((FullScreenMode)mode);
    }

    public void OnApplyPressed()
    {
        ApplySettings();
        SaveSettings();
    }

    void ApplySettings()
    {
        Resolution res = displayResolution.GetSelectedResolution();
        Screen.SetResolution(res.width, res.height, displayMode.SelectedMode);
    }

    void SaveSettings()
    {
        PlayerPrefs.SetInt("ResolutionIndex", displayResolution.SelectedResolutionIndex);
        PlayerPrefs.SetInt("DisplayMode", (int)displayMode.SelectedMode);
        PlayerPrefs.Save();
    }

    public void OnBackPressed()
    {
        GameManager.Instance.BackToPreviousScene();
    }
}
