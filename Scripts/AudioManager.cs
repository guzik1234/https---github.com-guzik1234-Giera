using UnityEngine;
using System.Collections;

/// <summary>
/// Manager audio - zarządza dźwiękami i muzyką
/// UWAGA: Używa ZAAWANSOWANYCH proceduralnych dźwięków (symulacja rzeczywistych)
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
        // Ostry, krótki dźwięk z harmonicznymi - symulacja uderzenia w drewno
        StartCoroutine(PlayComplexTone(new float[] { 800f, 1200f, 1600f }, 0.08f, 0.5f));
    }

    public void PlayBallHitBrick()
    {
        if (!enableAudio || audioSource == null) return;
        // Średni ton z rezonansem - symulacja uderzenia w ceramikę
        StartCoroutine(PlayComplexTone(new float[] { 500f, 750f, 1000f }, 0.12f, 0.4f));
    }

    public void PlayBallHitWall()
    {
        if (!enableAudio || audioSource == null) return;
        // Niski, tępy dźwięk - symulacja odbicia od ściany
        StartCoroutine(PlayComplexTone(new float[] { 300f, 450f }, 0.06f, 0.3f));
    }

    public void PlayBrickDestroy()
    {
        if (!enableAudio || audioSource == null) return;
        // Dźwięk rozbicia z szumem białym - symulacja pęknięcia
        StartCoroutine(PlayCrashSound(0.2f, 0.5f));
    }

    public void PlayLoseLife()
    {
        if (!enableAudio || audioSource == null) return;
        // Spadający ton z vibrato - smutny efekt
        StartCoroutine(PlaySweepDownWithVibrato(600f, 200f, 0.5f, 0.6f));
    }

    public void PlayGameOver()
    {
        if (!enableAudio || audioSource == null) return;
        // Głęboki, dramatyczny spadek z rezonansem
        StartCoroutine(PlayDramaticGameOver(0.8f, 0.7f));
    }

    public void PlayVictory()
    {
        if (!enableAudio || audioSource == null) return;
        // Wznoszący się akord major - radosna melodia
        StartCoroutine(PlayVictoryFanfare(1.0f, 0.8f));
    }

    public void PlayButtonClick()
    {
        if (!enableAudio || audioSource == null) return;
        // Krótki, przyjemny klik
        PlayTone(600f, 0.04f, 0.2f);
    }

    // Generator tonu z wieloma częstotliwościami (harmoniczne)
    private IEnumerator PlayComplexTone(float[] frequencies, float duration, float volume)
    {
        int sampleRate = 44100;
        int samples = Mathf.CeilToInt(sampleRate * duration);
        float[] data = new float[samples];
        
        for (int i = 0; i < samples; i++)
        {
            float t = i / (float)sampleRate;
            float envelope = 1f - (t / duration); // Zanikanie
            float sample = 0f;
            
            // Suma harmonicznych
            for (int f = 0; f < frequencies.Length; f++)
            {
                float amplitude = 1f / (f + 1); // Harmoniczne są cichsze
                sample += Mathf.Sin(2 * Mathf.PI * frequencies[f] * t) * amplitude;
            }
            
            data[i] = (sample / frequencies.Length) * volume * envelope;
        }
        
        AudioClip clip = AudioClip.Create("ComplexTone", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        audioSource.PlayOneShot(clip);
        yield return null;
    }

    // Dźwięk rozbicia z szumem (crash/shatter)
    private IEnumerator PlayCrashSound(float duration, float volume)
    {
        int sampleRate = 44100;
        int samples = Mathf.CeilToInt(sampleRate * duration);
        float[] data = new float[samples];
        
        for (int i = 0; i < samples; i++)
        {
            float t = i / (float)sampleRate;
            float progress = t / duration;
            
            // Szum biały (random) + wysokie częstotliwości
            float noise = Random.Range(-1f, 1f);
            float tone = Mathf.Sin(2 * Mathf.PI * Random.Range(2000f, 4000f) * t);
            
            // Szybkie zanikanie
            float envelope = Mathf.Pow(1f - progress, 2f);
            
            data[i] = (noise * 0.7f + tone * 0.3f) * volume * envelope;
        }
        
        AudioClip clip = AudioClip.Create("CrashSound", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        audioSource.PlayOneShot(clip);
        yield return null;
    }

    // Sweep w dół z vibrato (dramatyczny efekt)
    private IEnumerator PlaySweepDownWithVibrato(float startFreq, float endFreq, float duration, float volume)
    {
        int sampleRate = 44100;
        int samples = Mathf.CeilToInt(sampleRate * duration);
        float[] data = new float[samples];
        
        for (int i = 0; i < samples; i++)
        {
            float t = i / (float)sampleRate;
            float progress = t / duration;
            
            // Główna częstotliwość (sweep)
            float freq = Mathf.Lerp(startFreq, endFreq, progress);
            
            // Vibrato (modulacja)
            float vibrato = Mathf.Sin(2 * Mathf.PI * 6f * t) * 20f;
            
            float envelope = 1f - progress;
            data[i] = Mathf.Sin(2 * Mathf.PI * (freq + vibrato) * t) * volume * envelope;
        }
        
        AudioClip clip = AudioClip.Create("SweepDownVibrato", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        audioSource.PlayOneShot(clip);
        yield return null;
    }

    // Dramatyczny Game Over (akord minor)
    private IEnumerator PlayDramaticGameOver(float duration, float volume)
    {
        int sampleRate = 44100;
        int samples = Mathf.CeilToInt(sampleRate * duration);
        float[] data = new float[samples];
        
        // Akord minor (smutny): C, Eb, G
        float[] chord = { 261.63f, 311.13f, 392.00f };
        
        for (int i = 0; i < samples; i++)
        {
            float t = i / (float)sampleRate;
            float progress = t / duration;
            
            float sample = 0f;
            foreach (float freq in chord)
            {
                // Każda nuta opada
                float fallingFreq = freq * (1f - progress * 0.3f);
                sample += Mathf.Sin(2 * Mathf.PI * fallingFreq * t);
            }
            
            float envelope = 1f - (progress * 0.5f);
            data[i] = (sample / chord.Length) * volume * envelope;
        }
        
        AudioClip clip = AudioClip.Create("GameOverDramatic", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        audioSource.PlayOneShot(clip);
        yield return null;
    }

    // Fanfara zwycięstwa (akord major + arpeggio)
    private IEnumerator PlayVictoryFanfare(float duration, float volume)
    {
        int sampleRate = 44100;
        int samples = Mathf.CeilToInt(sampleRate * duration);
        float[] data = new float[samples];
        
        // Akord major (radosny): C, E, G, C
        float[] notes = { 523.25f, 659.25f, 783.99f, 1046.50f };
        
        for (int i = 0; i < samples; i++)
        {
            float t = i / (float)sampleRate;
            float progress = t / duration;
            
            // Arpeggio - nuty grają kolejno
            int noteIndex = Mathf.FloorToInt(progress * notes.Length) % notes.Length;
            float freq = notes[noteIndex];
            
            // Dodaj harmoniczne
            float sample = Mathf.Sin(2 * Mathf.PI * freq * t);
            sample += Mathf.Sin(2 * Mathf.PI * freq * 2f * t) * 0.5f; // Oktawa wyżej
            
            float envelope = Mathf.Sin(progress * Mathf.PI); // Bell curve
            data[i] = sample * volume * envelope * 0.5f;
        }
        
        AudioClip clip = AudioClip.Create("VictoryFanfare", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        audioSource.PlayOneShot(clip);
        yield return null;
    }

    // Generator prostego tonu (helper)
    private void PlayTone(float frequency, float duration, float volume)
    {
        int sampleRate = 44100;
        int samples = Mathf.CeilToInt(sampleRate * duration);
        float[] data = new float[samples];
        
        for (int i = 0; i < samples; i++)
        {
            float t = i / (float)sampleRate;
            float envelope = 1f - (t / duration);
            data[i] = Mathf.Sin(2 * Mathf.PI * frequency * t) * volume * envelope;
        }
        
        AudioClip clip = AudioClip.Create("Tone", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        audioSource.PlayOneShot(clip);
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
