using UnityEngine;
using System.Collections;

/// <summary>
/// Manager audio - zarządza dźwiękami i muzyką
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private float masterVolume = 0.3f;
    [SerializeField] private bool enableAudio = true;
    
    [Header("Sound Clips")]
    [SerializeField] private AudioClip paddleHitSound;
    [SerializeField] private AudioClip brickBreakSound;
    [SerializeField] private AudioClip wallBounceSound;
    [SerializeField] private AudioClip lifeLostSound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip victorySound;
    
    private AudioSource audioSource;

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
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = masterVolume;
        
        // Automatyczne ładowanie dźwięków z folderu Audio
        LoadAudioClips();
    }

    private void LoadAudioClips()
    {
        #if UNITY_EDITOR
        // W edytorze - ładuj z Assets/Audio
        if (paddleHitSound == null)
            paddleHitSound = LoadAudioFromPath("Assets/Audio/paddle_hit");
        if (brickBreakSound == null)
            brickBreakSound = LoadAudioFromPath("Assets/Audio/brick_break");
        if (wallBounceSound == null)
            wallBounceSound = LoadAudioFromPath("Assets/Audio/wall_bounce");
        if (lifeLostSound == null)
            lifeLostSound = LoadAudioFromPath("Assets/Audio/life_lost");
        if (gameOverSound == null)
            gameOverSound = LoadAudioFromPath("Assets/Audio/game_over");
        if (victorySound == null)
            victorySound = LoadAudioFromPath("Assets/Audio/victory");
        #else
        // W buildzie - próbuj załadować z Resources
        if (paddleHitSound == null)
            paddleHitSound = Resources.Load<AudioClip>("Audio/paddle_hit");
        if (brickBreakSound == null)
            brickBreakSound = Resources.Load<AudioClip>("Audio/brick_break");
        if (wallBounceSound == null)
            wallBounceSound = Resources.Load<AudioClip>("Audio/wall_bounce");
        if (lifeLostSound == null)
            lifeLostSound = Resources.Load<AudioClip>("Audio/life_lost");
        if (gameOverSound == null)
            gameOverSound = Resources.Load<AudioClip>("Audio/game_over");
        if (victorySound == null)
            victorySound = Resources.Load<AudioClip>("Audio/victory");
        #endif
            
        Debug.Log($"AudioManager: Loaded sounds - Paddle:{paddleHitSound!=null}, Brick:{brickBreakSound!=null}, Wall:{wallBounceSound!=null}, Life:{lifeLostSound!=null}, GameOver:{gameOverSound!=null}, Victory:{victorySound!=null}");
    }
    
    #if UNITY_EDITOR
    private AudioClip LoadAudioFromPath(string basePath)
    {
        // Szukaj pliku z różnymi rozszerzeniami
        string[] extensions = { ".wav", ".wav.wav", ".mp3", ".wav.mp3", ".ogg" };
        
        foreach (string ext in extensions)
        {
            string fullPath = basePath + ext;
            AudioClip clip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(fullPath);
            if (clip != null)
            {
                Debug.Log($"Loaded audio: {fullPath}");
                return clip;
            }
        }
        
        Debug.LogWarning($"Could not find audio file: {basePath}");
        return null;
    }
    #endif

    // Metody publiczne do odtwarzania dźwięków
    public void PlayBallHitPaddle()
    {
        if (!enableAudio || audioSource == null) return;
        if (paddleHitSound != null)
        {
            audioSource.PlayOneShot(paddleHitSound, masterVolume);
        }
        else
        {
            Debug.LogWarning("Paddle hit sound not loaded!");
        }
    }

    public void PlayBallHitBrick()
    {
        if (!enableAudio || audioSource == null) return;
        if (brickBreakSound != null)
        {
            audioSource.PlayOneShot(brickBreakSound, masterVolume * 0.8f);
        }
        else
        {
            Debug.LogWarning("Brick break sound not loaded!");
        }
    }

    public void PlayBallHitWall()
    {
        if (!enableAudio || audioSource == null) return;
        if (wallBounceSound != null)
        {
            audioSource.PlayOneShot(wallBounceSound, masterVolume * 0.6f);
        }
        else
        {
            Debug.LogWarning("Wall bounce sound not loaded!");
        }
    }

    public void PlayBrickDestroy()
    {
        if (!enableAudio || audioSource == null) return;
        if (brickBreakSound != null)
        {
            audioSource.PlayOneShot(brickBreakSound, masterVolume);
        }
        else
        {
            Debug.LogWarning("Brick destroy sound not loaded!");
        }
    }

    public void PlayLoseLife()
    {
        if (!enableAudio || audioSource == null) return;
        if (lifeLostSound != null)
        {
            audioSource.PlayOneShot(lifeLostSound, masterVolume);
        }
        else
        {
            Debug.LogWarning("Life lost sound not loaded!");
        }
    }

    public void PlayGameOver()
    {
        if (!enableAudio || audioSource == null) return;
        if (gameOverSound != null)
        {
            audioSource.PlayOneShot(gameOverSound, masterVolume * 4f);
        }
        else
        {
            Debug.LogWarning("Game over sound not loaded!");
        }
    }

    public void PlayVictory()
    {
        if (!enableAudio || audioSource == null) return;
        if (victorySound != null)
        {
            audioSource.PlayOneShot(victorySound, masterVolume);
        }
        else
        {
            Debug.LogWarning("Victory sound not loaded!");
        }
    }

    public void PlayButtonClick()
    {
        if (!enableAudio || audioSource == null) return;
        if (paddleHitSound != null)
        {
            audioSource.PlayOneShot(paddleHitSound, masterVolume * 0.3f);
        }
    }

    // Metody do zmiany głośności
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        if (audioSource != null)
        {
            audioSource.volume = masterVolume;
        }
    }

    public void ToggleAudio()
    {
        enableAudio = !enableAudio;
    }
}
