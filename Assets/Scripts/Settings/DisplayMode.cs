using UnityEngine;
using UnityEngine.UI;

public class DisplayMode : MonoBehaviour
{
    [Header("Buttons")]
    public Button fullscreenButton;
    public Button windowedButton;
    public Button borderlessButton;

    public FullScreenMode SelectedMode { get; private set; }

    void Awake()
    {
        // Safety default, will be overridden by SetFromSaved
        SelectedMode = FullScreenMode.FullScreenWindow;
        UpdateButtonStates();
    }

    // Called by SettingsMenu when menu opens
    public void SetFromSaved(FullScreenMode mode)
    {
        SelectedMode = mode;
        UpdateButtonStates();
    }

    // Button callbacks
    public void OnFullscreenPressed()
    {
        SelectedMode = FullScreenMode.ExclusiveFullScreen;
        UpdateButtonStates();
    }

    public void OnWindowedPressed()
    {
        SelectedMode = FullScreenMode.Windowed;
        UpdateButtonStates();
    }

    public void OnBorderlessPressed()
    {
        SelectedMode = FullScreenMode.FullScreenWindow;
        UpdateButtonStates();
    }

    void UpdateButtonStates()
    {
        SetButtonState(fullscreenButton, SelectedMode == FullScreenMode.ExclusiveFullScreen);
        SetButtonState(windowedButton, SelectedMode == FullScreenMode.Windowed);
        SetButtonState(borderlessButton, SelectedMode == FullScreenMode.FullScreenWindow);
    }

    void SetButtonState(Button button, bool isActive)
    {
        button.interactable = !isActive;

        // Optional: visual clarity if disabled colors are subtle
        //var colors = button.colors;
        //colors.disabledColor = colors.normalColor * 0.7f;
        //button.colors = colors;
    }
}
