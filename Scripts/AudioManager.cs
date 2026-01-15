using UnityEngine;

/// <summary>
/// Manager audio - zarządza dźwiękami i muzyką
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Clips")]
    [SerializeField] private AudioClip ballHitPaddleSound;
    [SerializeField] private AudioClip ballHitBrickSound;
    [SerializeField] private AudioClip ballHitWallSound;
    [SerializeField] private AudioClip brickDestroySound;
    [SerializeField] private AudioClip loseLifeSound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip victorySound;
    [SerializeField] private AudioClip buttonClickSound;
    
    [Header("Music")]
    [SerializeField] private AudioClip gameplayMusic;
    [SerializeField] private AudioClip menuMusic;
    
    [Header("Settings")]
    [SerializeField] private float masterVolume = 1f;
    [SerializeField] private float sfxVolume = 1f;
    [SerializeField] private float musicVolume = 0.5f;
    
    private AudioSource musicSource;
    private AudioSource sfxSource;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSources()
    {
        // Music source
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.volume = musicVolume * masterVolume;
        musicSource.playOnAwake = false;
        
        // SFX source
        sfxSource = GetComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.volume = sfxVolume * masterVolume;
        sfxSource.playOnAwake = false;
    }

    // Metody publiczne do odtwarzania dźwięków
    public void PlayBallHitPaddle()
    {
        PlaySFX(ballHitPaddleSound);
    }

    public void PlayBallHitBrick()
    {
        PlaySFX(ballHitBrickSound);
    }

    public void PlayBallHitWall()
    {
        PlaySFX(ballHitWallSound);
    }

    public void PlayBrickDestroy()
    {
        PlaySFX(brickDestroySound, 0.8f);
    }

    public void PlayLoseLife()
    {
        PlaySFX(loseLifeSound);
    }

    public void PlayGameOver()
    {
        PlaySFX(gameOverSound);
    }

    public void PlayVictory()
    {
        PlaySFX(victorySound);
    }

    public void PlayButtonClick()
    {
        PlaySFX(buttonClickSound);
    }

    private void PlaySFX(AudioClip clip, float volumeMultiplier = 1f)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, volumeMultiplier * sfxVolume * masterVolume);
        }
    }

    public void PlayMusic(AudioClip music)
    {
        if (musicSource != null && music != null)
        {
            if (musicSource.clip != music)
            {
                musicSource.clip = music;
                musicSource.Play();
            }
        }
    }

    public void PlayGameplayMusic()
    {
        PlayMusic(gameplayMusic);
    }

    public void PlayMenuMusic()
    {
        PlayMusic(menuMusic);
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    // Settery dla ustawień audio
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }

    private void UpdateVolumes()
    {
        if (musicSource != null)
        {
            musicSource.volume = musicVolume * masterVolume;
        }
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume * masterVolume;
        }
    }
}
