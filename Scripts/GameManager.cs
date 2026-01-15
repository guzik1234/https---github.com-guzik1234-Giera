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
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
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
        // Znajdź wszystkie bloki w scenie
        BrickController[] bricks = FindObjectsOfType<BrickController>();
        activeBricks.AddRange(bricks);
        
        // Znajdź piłkę
        currentBall = FindObjectOfType<BallController>();
        
        UpdateUI();
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
        currentLives--;
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
    {
        if (uiManager != null)
        {
            uiManager.UpdateScore(currentScore);
            uiManager.UpdateLives(currentLives);
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
