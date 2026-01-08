using UnityEngine;
using UnityEngine.UI;

public class AudioSlidersUI : MonoBehaviour
{
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    //[SerializeField] private Slider voiceSlider;

    [Header("Default slider values (0..1)")]
    [Range(0f, 1f)][SerializeField] private float masterDefault = 1f;
    [Range(0f, 1f)][SerializeField] private float musicDefault  = 1f;
    [Range(0f, 1f)][SerializeField] private float sfxDefault    = 1f;
    //[Range(0f, 1f)][SerializeField] private float voiceDefault  = 1f;

    private void Start()
    {
        var am = AudioManager.Instance;
        if (am == null)
        {
            Debug.LogError("AudioManager.Instance is null. Make sure AudioManager exists in the scene (or is DontDestroyOnLoad).");
            return;
        }

        // Set initial slider values without triggering events
        if (masterSlider) masterSlider.SetValueWithoutNotify(masterDefault);
        if (musicSlider)  musicSlider.SetValueWithoutNotify(musicDefault);
        if (sfxSlider)    sfxSlider.SetValueWithoutNotify(sfxDefault);
        //if (voiceSlider)  voiceSlider.SetValueWithoutNotify(voiceDefault);

        // Apply default volumes
        am.SetMasterVolume01(masterDefault);
        am.SetMusicVolume01(musicDefault);
        am.SetSfxVolume01(sfxDefault);
        am.SetVoiceVolume01(sfxDefault);

        // Add listeners to sliders
        if (masterSlider) masterSlider.onValueChanged.AddListener(am.SetMasterVolume01);
        if (musicSlider)  musicSlider.onValueChanged.AddListener(am.SetMusicVolume01);
        if (sfxSlider)    sfxSlider.onValueChanged.AddListener(am.SetSfxVolume01);
        if (sfxSlider)  sfxSlider.onValueChanged.AddListener(am.SetVoiceVolume01);
    }
}
