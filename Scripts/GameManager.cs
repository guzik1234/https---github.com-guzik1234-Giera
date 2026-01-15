using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// Główny manager gry - Singleton zarządzający stanem gry, punktacją i logiką
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    [SerializeField] private int startingLives = 3;
    [SerializeField] private int currentLevel = 1;
    
    [Header("References")]
    [SerializeField] private BallController ballPrefab;
    [SerializeField] private Transform ballSpawnPoint;
    [SerializeField] private GameObject brickContainer;
    
    [Header("UI References")]
    [SerializeField] private UIManager uiManager;
    
    private int currentScore = 0;
    private int currentLives;
    private List<BrickController> activeBricks = new List<BrickController>();
    private BallController currentBall;
    private bool isGameActive = false;
    private bool isPaused = false;

    void Awake()
    {
        // Singleton pattern - natychmiast niszcz starą instancję
        if (Instance != null && Instance != this)
        {
            Debug.Log($"GameManager.Awake() - Destroying old instance: {Instance.gameObject.name}");
            GameObject oldObject = Instance.gameObject;
            Instance = null; // Wyczyść najpierw referencję
            DestroyImmediate(oldObject); // Potem zniszcz obiekt
        }
        
        Instance = this;
        Debug.Log($"GameManager.Awake() - New Instance set to {gameObject.name}");
    }

    void OnDestroy()
    {
        // Wyczyść singleton gdy obiekt jest niszczony
        if (Instance == this)
        {
            Instance = null;
            Debug.Log("GameManager.OnDestroy() - Instance cleared");
        }
    }

    void Start()
    {
        // Wczytaj konfigurację poziomu
        var levelConfig = LevelSelector.GetCurrentConfig();
        if (levelConfig != null)
        {
            startingLives = levelConfig.startingLives;
            Debug.Log($"Starting with {startingLives} lives (Difficulty: {levelConfig.levelName})");
        }
        
        currentLives = startingLives;
        InitializeGame();
    }

    void Update()
    {
        // Obsługa pauzy (ESC) - kompatybilne ze starym i nowym Input System
        bool escapePressed = false;
        
        #if ENABLE_INPUT_SYSTEM
        if (UnityEngine.InputSystem.Keyboard.current != null)
        {
            escapePressed = UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame;
        }
        #else
        escapePressed = Input.GetKeyDown(KeyCode.Escape);
        #endif
        
        if (escapePressed && isGameActive)
        {
            PauseGame();
        }
    }

    private void InitializeGame()
    {
        // Reset score na początek nowej gry
        currentScore = 0;
        
        // WAŻNE: Wyczyść starą listę cegieł przed dodaniem nowych!
        activeBricks.Clear();
        
        // Znajdź UIManager jeśli jeszcze nie przypisany
        if (uiManager == null)
        {
            uiManager = FindFirstObjectByType<UIManager>();
            Debug.Log($"UIManager found: {(uiManager != null ? "YES" : "NO")}");
        }
        
        // Znajdź wszystkie bloki w scenie
        BrickController[] bricks = FindObjectsByType<BrickController>(FindObjectsSortMode.None);
        activeBricks.AddRange(bricks);
        
        // Znajdź piłkę
        currentBall = FindFirstObjectByType<BallController>();
        
        // Ustaw isGameActive na true
        isGameActive = true;
        
        // Aktualizuj UI wielokrotnie na początku (żeby UI zdążył się utworzyć)
        UpdateUI();
        InvokeRepeating(nameof(UpdateUI), 0.1f, 0.1f);
        Invoke(nameof(StopRepeatingUI), 2f);
        
        Debug.Log($"Game initialized: Lives={currentLives}, Bricks={activeBricks.Count}");
    }
    
    private void StopRepeatingUI()
    {
        CancelInvoke(nameof(UpdateUI));
        Debug.Log("UI updates stabilized");
    }

    public void StartGame()
    {
        isGameActive = true;
        if (currentBall != null)
        {
            currentBall.LaunchBall();
        }
    }

    public void AddScore(int points)
    {
        currentScore += points;
        UpdateUI();
    }

    public void OnBrickDestroyed(BrickController brick)
    {
        activeBricks.Remove(brick);
        
        // Sprawdź warunek wygranej
        if (activeBricks.Count == 0)
        {
            WinLevel();
        }
    }

    public void OnBallLost()
    {
        Debug.Log($"OnBallLost called! Lives before: {currentLives}");
        
        // Upewnij się, że UIManager jest dostępny
        if (uiManager == null)
        {
            uiManager = FindFirstObjectByType<UIManager>();
        }
        
        currentLives--;
        Debug.Log($"OnBallLost - Lives after: {currentLives}");
        UpdateUI();
        
        // Odtwórz dźwięk utraty życia
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayLoseLife();
        }
        
        if (currentLives <= 0)
        {
            GameOver();
        }
        else
        {
            // Respawn piłki
            Invoke(nameof(RespawnBall), 1f);
        }
    }

    private void RespawnBall()
    {
        if (currentBall != null)
        {
            currentBall.ResetBall();
            Invoke(nameof(StartGame), 1f);
        }
    }

    private void WinLevel()
    {
        isGameActive = false;
        Time.timeScale = 0f; // Zatrzymaj grę
        
        // Odtwórz dźwięk zwycięstwa
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayVictory();
        }
        
        if (uiManager != null)
        {
            uiManager.ShowVictoryScreen(currentScore);
        }
    }

    private void GameOver()
    {
        isGameActive = false;
        Time.timeScale = 0f; // Zatrzymaj grę
        
        // Odtwórz dźwięk Game Over
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameOver();
        }
        
        if (uiManager != null)
        {
            uiManager.ShowGameOverScreen(currentScore);
        }
    }

    private void UpdateUI()
    {        // Jeśli UIManager jest null, spróbuj go znaleźć
        if (uiManager == null)
        {
            uiManager = FindFirstObjectByType<UIManager>();
        }
                if (uiManager != null)
        {
            uiManager.UpdateScore(currentScore);
            uiManager.UpdateLives(currentLives);
            Debug.Log($"UI Updated: Score={currentScore}, Lives={currentLives}");
        }
        else
        {
            Debug.LogWarning("UIManager is NULL - cannot update UI!");
        }
    }

    public void PauseGame()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        
        if (uiManager != null)
        {
            uiManager.ShowPauseMenu(isPaused);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    // Getters publiczne
    public int GetCurrentScore() => currentScore;
    public int GetCurrentLives() => currentLives;
    public bool IsGameActive() => isGameActive;
}
