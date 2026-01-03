using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayResolution : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;

    // base 16:9 ladder widths
    readonly int[] baseWidths = { 1280, 1600, 1920, 2560, 3840 };

    List<Resolution> ladderResolutions = new();

    public int SelectedResolutionIndex { get; private set; }

    void Awake()
    {
        BuildLadderList();
    }

    void BuildLadderList()
    {
        resolutionDropdown.ClearOptions();
        ladderResolutions.Clear();

        float aspect = (float)Screen.currentResolution.width / Screen.currentResolution.height;

        List<string> options = new();

        foreach (int width in baseWidths)
        {
            int height = Mathf.RoundToInt(width / aspect);

            Resolution r = new Resolution { width = width, height = height, refreshRate = 0 };
            ladderResolutions.Add(r);
            options.Add($"{width} x {height}");
        }

        resolutionDropdown.AddOptions(options);

        resolutionDropdown.onValueChanged.RemoveAllListeners();
        resolutionDropdown.onValueChanged.AddListener(i => SelectedResolutionIndex = i);

        SelectedResolutionIndex = 0;
        resolutionDropdown.value = 0;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetFromSaved(int savedIndex)
    {
        SelectedResolutionIndex = Mathf.Clamp(savedIndex, 0, ladderResolutions.Count - 1);
        resolutionDropdown.value = SelectedResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public Resolution GetSelectedResolution()
    {
        return ladderResolutions[SelectedResolutionIndex];
    }
}
