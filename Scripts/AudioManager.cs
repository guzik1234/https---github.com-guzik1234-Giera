using UnityEngine;

/// <summary>
/// Manager audio - zarządza dźwiękami i muzyką
/// UWAGA: Używa proceduralnych dźwięków - nie potrzebuje plików audio!
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private float masterVolume = 0.3f;
    [SerializeField] private bool enableAudio = true;
    
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
    }

    // Metody publiczne do odtwarzania dźwięków
    public void PlayBallHitPaddle()
    {
        if (!enableAudio || audioSource == null) return;
        // Krótki, ostry dźwięk - wyższy ton
        PlayTone(600f, 0.08f, 0.4f);
    }

    public void PlayBallHitBrick()
    {
        if (!enableAudio || audioSource == null) return;
        // Średni ton, krótki
        PlayTone(400f, 0.1f, 0.3f);
    }

    public void PlayBallHitWall()
    {
        if (!enableAudio || audioSource == null) return;
        // Niski ton, bardzo krótki
        PlayTone(300f, 0.05f, 0.25f);
    }

    public void PlayBrickDestroy()
    {
        if (!enableAudio || audioSource == null) return;
        // Wyższy dźwięk niszczenia
        PlayTone(800f, 0.15f, 0.35f);
    }

    public void PlayLoseLife()
    {
        if (!enableAudio || audioSource == null) return;
        // Spadający ton (przykro brzmiący)
        StartCoroutine(PlaySweepDown(500f, 150f, 0.4f, 0.5f));
    }

    public void PlayGameOver()
    {
        if (!enableAudio || audioSource == null) return;
        // Głęboki, smutny dźwięk
        StartCoroutine(PlaySweepDown(300f, 100f, 0.8f, 0.6f));
    }

    public void PlayVictory()
    {
        if (!enableAudio || audioSource == null) return;
        // Wznoszący się radosny dźwięk
        StartCoroutine(PlaySweepUp(400f, 800f, 0.6f, 0.7f));
    }

    public void PlayButtonClick()
    {
        if (!enableAudio || audioSource == null) return;
        PlayTone(500f, 0.05f, 0.2f);
    }

    // Generator prostego tonu
    private void PlayTone(float frequency, float duration, float volume)
    {
        int sampleRate = 44100;
        int samples = Mathf.CeilToInt(sampleRate * duration);
        float[] data = new float[samples];
        
        for (int i = 0; i < samples; i++)
        {
            float t = i / (float)sampleRate;
            float envelope = 1f - (t / duration); // Zanikanie
            data[i] = Mathf.Sin(2 * Mathf.PI * frequency * t) * volume * envelope;
        }
        
        AudioClip clip = AudioClip.Create("Tone", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        audioSource.PlayOneShot(clip);
    }

    // Sweep w dół (smutny dźwięk)
    private System.Collections.IEnumerator PlaySweepDown(float startFreq, float endFreq, float duration, float volume)
    {
        int sampleRate = 44100;
        int samples = Mathf.CeilToInt(sampleRate * duration);
        float[] data = new float[samples];
        
        for (int i = 0; i < samples; i++)
        {
            float t = i / (float)sampleRate;
            float progress = t / duration;
            float freq = Mathf.Lerp(startFreq, endFreq, progress);
            float envelope = 1f - progress;
            data[i] = Mathf.Sin(2 * Mathf.PI * freq * t) * volume * envelope;
        }
        
        AudioClip clip = AudioClip.Create("SweepDown", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        audioSource.PlayOneShot(clip);
        yield return null;
    }

    // Sweep w górę (radosny dźwięk)
    private System.Collections.IEnumerator PlaySweepUp(float startFreq, float endFreq, float duration, float volume)
    {
        int sampleRate = 44100;
        int samples = Mathf.CeilToInt(sampleRate * duration);
        float[] data = new float[samples];
        
        for (int i = 0; i < samples; i++)
        {
            float t = i / (float)sampleRate;
            float progress = t / duration;
            float freq = Mathf.Lerp(startFreq, endFreq, progress);
            float envelope = 1f - (progress * 0.5f); // Powolniejsze zanikanie
            data[i] = Mathf.Sin(2 * Mathf.PI * freq * t) * volume * envelope;
        }
        
        AudioClip clip = AudioClip.Create("SweepUp", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        audioSource.PlayOneShot(clip);
        yield return null;
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
