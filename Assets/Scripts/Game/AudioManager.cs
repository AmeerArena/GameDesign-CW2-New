using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private bool dontDestroyOnLoad = true;

    [Header("Music")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip tiledDayMusic;
    [SerializeField] private AudioClip tiledNightMusic;

    [Header("Optional Mixer")]
    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private string masterVolParam = "MasterVol";
    [SerializeField] private string musicVolParam  = "MusicVol";
    [SerializeField] private string sfxVolParam    = "SfxVol";
    [SerializeField] private string voiceVolParam  = "VoiceVol";
    [SerializeField] private string uiVolParam     = "UiVol";

    [Header("Audio Sources")]
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
        if (musicSource == null) musicSource = gameObject.AddComponent<AudioSource>();
        if (sfxSource == null)   sfxSource   = gameObject.AddComponent<AudioSource>();
        if (voiceSource == null) voiceSource = gameObject.AddComponent<AudioSource>();
        if (uiSource == null)    uiSource    = gameObject.AddComponent<AudioSource>();
    }

    void ConfigureDefaults()
    {
        musicSource.loop = true;
        musicSource.playOnAwake = false;

        sfxSource.playOnAwake = false;
        voiceSource.playOnAwake = false;
        uiSource.playOnAwake = false;
    }

    // One-shots 

    public void PlaySfx(AudioClip clip, float volume = 0.5f, float pitch = 1f)
        => PlayOneShot(sfxSource, clip, volume, pitch);

    public void PlayUISfx(AudioClip clip, float volume = 0.2f, float pitch = 1f)
        => PlayOneShot(uiSource, clip, volume, pitch);

    public void PlayVoice(AudioClip clip, float volume = 0.5f, float pitch = 1f)
        => PlayOneShot(voiceSource, clip, volume, pitch);

    static void PlayOneShot(AudioSource src, AudioClip clip, float volume, float pitch)
    {
        if (src == null || clip == null) return;
        src.pitch = pitch;
        src.PlayOneShot(clip, Mathf.Clamp01(volume));
    }

    // Music 

    public void PlayMusic(AudioClip clip, float volume = 0.05f)
    {
        if (musicSource == null || clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.volume = Mathf.Clamp01(volume);
        musicSource.Play();
    }

    public void PlayMenuMusic(float volume = 0.05f)
    {
        PlayMusic(menuMusic, volume);
    }

    public void PlayDayMusic(float volume = 0.05f)
    {
        PlayMusic(tiledDayMusic, volume);
    }

    public void PlayNightMusic(float volume = 0.05f)
    {
        PlayMusic(tiledNightMusic, volume);
    }

    public void StopMusic()
    {
        if (musicSource == null) return;
        musicSource.Stop();
        musicSource.clip = null;
    }

    public void FadeMusicTo(float targetVolume, float duration)
    {
        if (musicSource == null) return;

        if (duration <= 0f)
        {
            musicSource.volume = Mathf.Clamp01(targetVolume);
            return;
        }
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

    // Mixer volumes (optional) 

    public void SetMasterVolume01(float v) => SetMixerVolume01(masterVolParam, v);
    public void SetMusicVolume01(float v)  => SetMixerVolume01(musicVolParam, v);
    public void SetSfxVolume01(float v)    => SetMixerVolume01(sfxVolParam, v);
    public void SetVoiceVolume01(float v)  => SetMixerVolume01(voiceVolParam, v);
    public void SetUiVolume01(float v)     => SetMixerVolume01(uiVolParam, v);

    void SetMixerVolume01(string param, float value01)
    {
        if (masterMixer == null || string.IsNullOrEmpty(param)) return;
        masterMixer.SetFloat(param, ToDb(value01));
    }

    static float ToDb(float value01)
    {
        value01 = Mathf.Clamp01(value01);
        return value01 <= 0.0001f ? -80f : Mathf.Log10(value01) * 20f;
    }
}
