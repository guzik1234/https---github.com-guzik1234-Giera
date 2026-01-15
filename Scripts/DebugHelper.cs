using UnityEngine;

/// <summary>
/// Helper do debugowania - pokazuje logi w konsoli
/// </summary>
public class DebugHelper : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== DEBUG HELPER START ===");
        
        // Sprawdź GameManager
        var gm = FindObjectOfType<GameManager>();
        if (gm != null)
            Debug.Log("✓ GameManager found");
        else
            Debug.LogWarning("✗ GameManager NOT found");
        
        // Sprawdź UIManager
        var uim = FindObjectOfType<UIManager>();
        if (uim != null)
        {
            Debug.Log("✓ UIManager found");
            Debug.Log($"  - HUD Panel: {(uim.hudPanel != null ? "OK" : "NULL")}");
            Debug.Log($"  - Pause Panel: {(uim.pausePanel != null ? "OK" : "NULL")}");
            Debug.Log($"  - GameOver Panel: {(uim.gameOverPanel != null ? "OK" : "NULL")}");
            Debug.Log($"  - Victory Panel: {(uim.victoryPanel != null ? "OK" : "NULL")}");
            Debug.Log($"  - Score Text: {(uim.scoreText != null ? "OK" : "NULL")}");
            Debug.Log($"  - Lives Text: {(uim.livesText != null ? "OK" : "NULL")}");
        }
        else
            Debug.LogWarning("✗ UIManager NOT found");
        
        // Sprawdź EventSystem
        var es = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
        if (es != null)
            Debug.Log("✓ EventSystem found");
        else
            Debug.LogWarning("✗ EventSystem NOT found");
        
        // Sprawdź Canvas
        var canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
            Debug.Log($"✓ Canvas found - Children: {canvas.transform.childCount}");
        else
            Debug.LogWarning("✗ Canvas NOT found");
        
        Debug.Log("=== DEBUG HELPER END ===");
    }
}
