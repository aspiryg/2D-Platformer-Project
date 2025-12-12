using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Singleton instance
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] private AudioSource musicSource;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 1f;

    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 0.5f;


    // Initialize the AudioManager singleton
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        
        if (sfxSource == null)
        {
            // 
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false; 
        }

        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = true;
        }

        // initial volume settings
        sfxSource.volume = sfxVolume;
        musicSource.volume = musicVolume;
    }

    public void PlaySFX(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioManager: Attempted to play null audio clip");
            return;
        }

        //
        sfxSource.PlayOneShot(clip, volumeScale * sfxVolume);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioManager: Attempted to play null music clip");
            return;
        }

        // Stop current music before starting new one
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
    // 
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume); // Ensure volume is between 0 and 1
        sfxSource.volume = sfxVolume;
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
    }
}
