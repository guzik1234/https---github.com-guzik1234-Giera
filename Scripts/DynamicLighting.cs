using UnityEngine;

/// <summary>
/// Skrypt dla dynamicznego oświetlenia - reaguje na eventy gry
/// </summary>
public class DynamicLighting : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField] private Light mainLight;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hitColor = Color.cyan;
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private Color victoryColor = Color.green;
    
    [Header("Intensity")]
    [SerializeField] private float normalIntensity = 1f;
    [SerializeField] private float flashIntensity = 2f;
    [SerializeField] private float flashDuration = 0.1f;
    
    [Header("Pulse Effect")]
    [SerializeField] private bool pulseEnabled = true;
    [SerializeField] private float pulseSpeed = 1f;
    [SerializeField] private float pulseAmount = 0.2f;

    private Color targetColor;
    private float targetIntensity;
    private bool isFlashing = false;

    void Start()
    {
        if (mainLight == null)
        {
            mainLight = GetComponent<Light>();
        }
        
        targetColor = normalColor;
        targetIntensity = normalIntensity;
    }

    void Update()
    {
        if (!isFlashing && pulseEnabled)
        {
            // Pulsujące oświetlenie
            float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            mainLight.intensity = targetIntensity + pulse;
        }
        
        // Smooth transition koloru
        mainLight.color = Color.Lerp(mainLight.color, targetColor, Time.deltaTime * 5f);
    }

    public void FlashOnHit()
    {
        if (!isFlashing)
        {
            StartCoroutine(FlashEffect(hitColor));
        }
    }

    public void FlashOnDamage()
    {
        if (!isFlashing)
        {
            StartCoroutine(FlashEffect(damageColor));
        }
    }

    public void SetVictoryLighting()
    {
        targetColor = victoryColor;
        targetIntensity = flashIntensity;
    }

    private System.Collections.IEnumerator FlashEffect(Color flashColor)
    {
        isFlashing = true;
        Color originalColor = targetColor;
        float originalIntensity = targetIntensity;
        
        // Flash
        mainLight.color = flashColor;
        mainLight.intensity = flashIntensity;
        
        yield return new WaitForSeconds(flashDuration);
        
        // Return to normal
        targetColor = originalColor;
        targetIntensity = originalIntensity;
        isFlashing = false;
    }

    // Metody wywoływane przez inne skrypty
    public void OnBrickDestroyed()
    {
        FlashOnHit();
    }

    public void OnBallLost()
    {
        FlashOnDamage();
    }

    public void OnVictory()
    {
        SetVictoryLighting();
    }
}
