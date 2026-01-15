using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manager interfejsu użytkownika - zarządza wszystkimi elementami UI
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("HUD Elements")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;
    public GameObject hudPanel;
    
    [Header("Menu Panels")]
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject victoryPanel;
    
    [Header("End Game Elements")]
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI victoryScoreText;

    void Start()
    {
        // Ukryj wszystkie panele menu na start
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
        
        // Pokaż HUD
        if (hudPanel != null) hudPanel.SetActive(true);
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score:D6}";
        }
    }

    public void UpdateLives(int lives)
    {
        if (livesText != null)
        {
            livesText.text = $"Lives: {lives}";
        }
    }

    public void ShowPauseMenu(bool show)
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(show);
        }
        
        if (hudPanel != null)
        {
            hudPanel.SetActive(!show);
        }
    }

    public void ShowGameOverScreen(int finalScore)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            if (finalScoreText != null)
            {
                finalScoreText.text = $"Final Score: {finalScore:D6}";
            }
        }
        
        if (hudPanel != null)
        {
            hudPanel.SetActive(false);
        }
    }

    public void ShowVictoryScreen(int finalScore)
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
            
            if (victoryScoreText != null)
            {
                victoryScoreText.text = $"Victory! Score: {finalScore:D6}";
            }
        }
        
        if (hudPanel != null)
        {
            hudPanel.SetActive(false);
        }
    }

    // Metody wywoływane przez przyciski UI
    public void OnResumeButton()
    {
        GameManager.Instance.PauseGame();
    }

    public void OnRestartButton()
    {
        GameManager.Instance.RestartGame();
    }

    public void OnMainMenuButton()
    {
        GameManager.Instance.LoadMainMenu();
    }

    public void OnQuitButton()
    {
        GameManager.Instance.QuitGame();
    }
}
