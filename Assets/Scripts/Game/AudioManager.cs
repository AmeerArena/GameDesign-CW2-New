using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource sfxSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();

        if (sfxSource == null)
            sfxSource = gameObject.AddComponent<AudioSource>();

        sfxSource.playOnAwake = false;
        sfxSource.spatialBlend = 0f; // 2D
    }

    public void PlaySfx(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        sfxSource.Stop();
        sfxSource.PlayOneShot(clip, Mathf.Clamp01(volume));
    }
}
