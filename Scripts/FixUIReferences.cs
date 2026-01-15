using UnityEngine;
using TMPro;

/// <summary>
/// Naprawia referencje UI - automatycznie znajduje i przypisuje TextMeshPro komponenty
/// </summary>
public class FixUIReferences : MonoBehaviour
{
    void Start()
    {
        UIManager uiManager = GetComponent<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("FixUIReferences: UIManager not found on this GameObject!");
            return;
        }
        
        // Znajdź wszystkie TextMeshPro komponenty w hierarchii
        TextMeshProUGUI[] allTexts = GetComponentsInChildren<TextMeshProUGUI>(true);
        
        Debug.Log($"Found {allTexts.Length} TextMeshPro texts in UI hierarchy");
        
        foreach (var text in allTexts)
        {
            string name = text.gameObject.name.ToLower();
            
            if (name.Contains("score") && !name.Contains("final") && !name.Contains("victory"))
            {
                uiManager.scoreText = text;
                Debug.Log($"✓ Assigned scoreText: {text.gameObject.name}");
            }
            else if (name.Contains("lives") || name.Contains("life"))
            {
                uiManager.livesText = text;
                Debug.Log($"✓ Assigned livesText: {text.gameObject.name}");
            }
            else if (name.Contains("final") && name.Contains("score"))
            {
                uiManager.finalScoreText = text;
                Debug.Log($"✓ Assigned finalScoreText: {text.gameObject.name}");
            }
            else if (name.Contains("victory") && name.Contains("score"))
            {
                uiManager.victoryScoreText = text;
                Debug.Log($"✓ Assigned victoryScoreText: {text.gameObject.name}");
            }
        }
        
        // Znajdź panele
        Transform hudPanel = transform.Find("HUD Panel");
        if (hudPanel != null)
        {
            uiManager.hudPanel = hudPanel.gameObject;
            Debug.Log("✓ Assigned HUD Panel");
        }
        
        Transform pausePanel = transform.Find("Pause Panel");
        if (pausePanel != null)
        {
            uiManager.pausePanel = pausePanel.gameObject;
            Debug.Log("✓ Assigned Pause Panel");
        }
        
        Transform gameOverPanel = transform.Find("GameOver Panel");
        if (gameOverPanel != null)
        {
            uiManager.gameOverPanel = gameOverPanel.gameObject;
            Debug.Log("✓ Assigned GameOver Panel");
        }
        
        Transform victoryPanel = transform.Find("Victory Panel");
        if (victoryPanel != null)
        {
            uiManager.victoryPanel = victoryPanel.gameObject;
            Debug.Log("✓ Assigned Victory Panel");
        }
        
        // Wymuś aktualizację UI
        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            Debug.Log("Forcing UI update from FixUIReferences...");
        }
        
        // Usuń ten komponent po zakończeniu (nie jest już potrzebny)
        Destroy(this, 1f);
    }
}
