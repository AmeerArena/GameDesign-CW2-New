using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Persistence")]
    [SerializeField] private bool dontDestroyOnLoad = true;

    [Header("Optional Mixer (recommended later)")]
    [SerializeField] private AudioMixer masterMixer;
    [Tooltip("Name of exposed mixer param, e.g. MasterVol (in dB). Leave empty if not using.")]
    [SerializeField] private string masterVolParam = "MasterVol";
    [SerializeField] private string musicVolParam  = "MusicVol";
    [SerializeField] private string sfxVolParam    = "SfxVol";
    [SerializeField] private string voiceVolParam  = "VoiceVol";
    [SerializeField] private string uiVolParam     = "UiVol";

    [Header("Audio Sources (assign in Inspector)")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource voiceSource;
    public AudioSource uiSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (dontDestroyOnLoad)
            DontDestroyOnLoad(gameObject);

        EnsureSourcesExist();
        ConfigureDefaults();
    }

    void EnsureSourcesExist()
    {
        // If you forgot to add sources in the Inspector, we create them.
        if (musicSource == null) musicSource = gameObject.AddComponent<AudioSource>();
        if (sfxSource == null)   sfxSource   = gameObject.AddComponent<AudioSource>();
        if (voiceSource == null) voiceSource = gameObject.AddComponent<AudioSource>();
        if (uiSource == null)    uiSource    = gameObject.AddComponent<AudioSource>();
    }

    void ConfigureDefaults()
    {
        // Music
        musicSource.loop = true;
        musicSource.playOnAwake = false;

        // One-shots
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;

        voiceSource.loop = false;
        voiceSource.playOnAwake = false;

        uiSource.loop = false;
        uiSource.playOnAwake = false;
    }

    // -----------------------
    // Public API: SFX / UI / Voice
    // -----------------------

    public void PlaySfx(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        PlayOneShot(sfxSource, clip, volume, pitch);
    }

    public void PlayUISfx(AudioClip clip, float volume = 0.2f, float pitch = 1f)
    {
        PlayOneShot(uiSource, clip, volume, pitch);
    }

    public void PlayVoice(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        PlayOneShot(voiceSource, clip, volume, pitch);
    }

    static void PlayOneShot(AudioSource src, AudioClip clip, float volume, float pitch)
    {
        if (src == null || clip == null) return;

        // Pitch affects the source, so set it then play.
        src.pitch = pitch;
        src.PlayOneShot(clip, Mathf.Clamp01(volume));
    }

    // -----------------------
    // Public API: Music (now + future-proof)
    // -----------------------

    /// <summary>Starts looping music. If the same clip is already playing, does nothing.</summary>
    public void PlayMusic(AudioClip clip, float volume = 1f)
    {
        if (musicSource == null || clip == null) return;

        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.clip = clip;
        musicSource.volume = Mathf.Clamp01(volume);
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource == null) return;
        musicSource.Stop();
        musicSource.clip = null;
    }

    /// <summary>Simple fade helper you can use later for scene transitions.</summary>
    public void FadeMusicTo(float targetVolume, float duration)
    {
        if (musicSource == null) return;
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(musicSource, Mathf.Clamp01(targetVolume), duration));
    }

    System.Collections.IEnumerator FadeRoutine(AudioSource src, float target, float duration)
    {
        float start = src.volume;
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            src.volume = Mathf.Lerp(start, target, t / duration);
            yield return null;
        }

        src.volume = target;
    }

    // -----------------------
    // Mixer-based volume control (optional)
    // -----------------------

    public void SetMixerVolume01(string exposedParam, float value01)
    {
        if (masterMixer == null || string.IsNullOrEmpty(exposedParam)) return;
        masterMixer.SetFloat(exposedParam, ToDb(value01));
    }

    public void SetMasterVolume01(float v) => SetMixerVolume01(masterVolParam, v);
    public void SetMusicVolume01(float v)  => SetMixerVolume01(musicVolParam, v);
    public void SetSfxVolume01(float v)    => SetMixerVolume01(sfxVolParam, v);
    public void SetVoiceVolume01(float v)  => SetMixerVolume01(voiceVolParam, v);
    public void SetUiVolume01(float v)     => SetMixerVolume01(uiVolParam, v);

    static float ToDb(float value01)
    {
        // 0 -> -80dB (effectively silent), 1 -> 0dB
        value01 = Mathf.Clamp01(value01);
        if (value01 <= 0.0001f) return -80f;
        return Mathf.Log10(value01) * 20f;
    }
}
