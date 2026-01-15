using UnityEngine;

/// <summary>
/// Dodaje efekt glow do obiektu używając własnego shadera
/// </summary>
public class GlowEffect : MonoBehaviour
{
    [Header("Glow Settings")]
    [SerializeField] private bool useGlow = true;
    [SerializeField] private Color emissionColor = new Color(0f, 0.8f, 0.8f); // Ciemny turkusowy zamiast żółtego
    [SerializeField] [Range(0f, 2f)] private float emissionStrength = 0.5f;
    [SerializeField] [Range(0f, 20f)] private float pulseSpeed = 15f; // BARDZO SZYBKIE pulsowanie!
    [SerializeField] [Range(0f, 1f)] private float pulseAmount = 0.9f; // MAKSYMALNE pulsowanie!
    
    private Material glowMaterial;
    private Renderer objectRenderer;
    private Color originalColor;
    private Light glowLight; // Point Light dla widocznego pulsowania
    private float lastLogTime = 0f;

    /// <summary>
    /// Zachowuje oryginalny kolor - NIE zmienia RGB!
    /// </summary>
    private Color NormalizeColorBrightness(Color color)
    {
        // Zwracamy oryginalny kolor bez zmian - mnożnik zostanie zastosowany w Update
        return color;
    }

    void Start()
    {
        Debug.Log($"🔵 GlowEffect.Start() - Initializing on {gameObject.name}...");
        
        // NIE dodawaj GlowEffect do ścian!
        if (gameObject.CompareTag("Wall"))
        {
            Debug.Log("⚠️ Skipping GlowEffect for Wall object!");
            useGlow = false;
            return;
        }
        
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer == null)
        {
            Debug.LogError("❌ GlowEffect: No Renderer found!");
            return;
        }

        Debug.Log($"✓ Renderer found, useGlow={useGlow}, pulseSpeed={pulseSpeed}");
        
        if (useGlow)
        {
            ApplyGlowShader();
            CreateGlowLight(); // Dodaj światło dla widocznego efektu
        }
        else
        {
            Debug.LogWarning("⚠️ useGlow=false, skipping shader");
        }
    }
    
    /// <summary>
    /// Tworzy Point Light który pulsuje wraz z emission - WIDOCZNY efekt!
    /// </summary>
    private void CreateGlowLight()
    {
        // Dodaj Light component
        glowLight = gameObject.AddComponent<Light>();
        glowLight.type = LightType.Point;
        glowLight.color = emissionColor;
        glowLight.range = 4f; // WIĘKSZY zasięg światła!
        glowLight.intensity = 4f; // MAKSYMALNA jasność!
        glowLight.shadows = LightShadows.None; // Bez cieni dla performance
        
        Debug.Log($"💡 Point Light added: color={emissionColor}, range={glowLight.range}, intensity={glowLight.intensity}");
    }
    
    void Update()
    {
        // NIE pulsuj ścian!
        if (gameObject.CompareTag("Wall"))
        {
            return;
        }
        
        // Pulsowanie emission w Update() - zawsze działa!
        if (glowMaterial != null && useGlow && pulseSpeed > 0)
        {
            float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseAmount + 1f;
            
            // Oblicz luminance koloru
            float luminance = 0.299f * emissionColor.r + 0.587f * emissionColor.g + 0.114f * emissionColor.b;
            
            // Ciemne kolory (czerwony, pomarańczowy) dostają WYŻSZY mnożnik!
            float colorBoost = 1f;
            if (luminance < 0.25f) // Bardzo ciemne (np. ciemnoczerwony 0.6,0,0 = luminance 0.179)
            {
                colorBoost = 5.5f; // 5.5x BARDZO MOCNO dla ciemnych!
            }
            else if (luminance < 0.4f) // Czerwony (~0.299)
            {
                colorBoost = 2.5f; // 2.5x mocniej!
            }
            else if (luminance < 0.6f) // Pomarańczowy
            {
                colorBoost = 2.0f; // 2x mocniej!
            }
            else if (luminance < 0.75f) // Zielony
            {
                colorBoost = 1.5f; // 1.5x mocniej!
            }
            else if (luminance < 0.9f) // Żółty (~0.886) - też boost!
            {
                colorBoost = 1.3f; // Łagodny boost żeby nie był biały
            }
            // Niebieski (~0.114) - standardowy mnożnik
            
            Color emissionFinal = emissionColor * emissionStrength * pulse * 80f * colorBoost;
            glowMaterial.SetColor("_EmissionColor", emissionFinal);
            
            // Pulsuj Light intensity - TO BĘDZIE WIDOCZNE!
            if (glowLight != null)
            {
                glowLight.intensity = pulse * 8.0f; // EKSTREMALNE pulsowanie światła! (0.8 do 15.2)
            }
            
            // Log co 2 sekundy żeby zobaczyć czy Update działa
            if (Time.time - lastLogTime > 2f)
            {
                Debug.Log($"⚡ GlowEffect.Update() pulse={pulse:F2}, emission={emissionFinal}, light={glowLight?.intensity:F2}");
                lastLogTime = Time.time;
            }
        }
    }

    private void ApplyGlowShader()
    {
        Debug.Log("🟡 ApplyGlowShader() called");
        
        // UŻYJ ISTNIEJĄCEGO MATERIAŁU zamiast tworzyć nowy!
        glowMaterial = objectRenderer.material;
        
        if (glowMaterial != null)
        {
            Debug.Log($"✓ Using existing material: {glowMaterial.shader.name}");
            
            // Zachowaj oryginalny kolor
            originalColor = glowMaterial.color;
            Debug.Log($"✓ Original color: {originalColor}");
            
            // WŁĄCZ EMISSION na istniejącym materiale
            glowMaterial.EnableKeyword("_EMISSION");
            glowMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            
            // Oblicz luminance i zastosuj boost dla ciemnych kolorów
            float luminance = 0.299f * emissionColor.r + 0.587f * emissionColor.g + 0.114f * emissionColor.b;
            float colorBoost = 1f;
            if (luminance < 0.25f) colorBoost = 5.5f; // Bardzo ciemne (ciemnoczerwony) - BARDZO MOCNO!
            else if (luminance < 0.4f) colorBoost = 2.5f; // Czerwony
            else if (luminance < 0.6f) colorBoost = 2.0f; // Pomarańczowy
            else if (luminance < 0.75f) colorBoost = 1.5f; // Zielony
            else if (luminance < 0.9f) colorBoost = 1.3f; // Żółty - też boost!
            
            // Ustaw kolor emission (jaśniejszy niż base color) - HDR z dynamicznym boostem!
            Color emissionFinal = emissionColor * emissionStrength * 80f * colorBoost;
            glowMaterial.SetColor("_EmissionColor", emissionFinal);
            
            Debug.Log($"✓ Emission enabled on existing material! Color: {emissionFinal} (HDR x20)");
            Debug.Log($"🎨 Glow ready! useGlow={useGlow}, pulseSpeed={pulseSpeed}, pulseAmount={pulseAmount}");
        }
        else
        {
            Debug.LogError("❌ objectRenderer.material is NULL!");
        }
    }

    // Metoda do zmiany koloru glow w runtime
    public void SetGlowColor(Color color)
    {
        Debug.Log($"🟢 SetGlowColor() called with color: {color}");
        
        // NORMALIZUJ JASNOŚĆ koloru, aby wszystkie kolory pulsowały jednakowo!
        Color normalizedColor = NormalizeColorBrightness(color);
        Debug.Log($"   Normalized color: {normalizedColor} (was: {color})");
        
        emissionColor = normalizedColor;
        useGlow = true; // Włącz glow
        pulseSpeed = 12f; // BARDZO SZYBKIE pulsowanie!
        pulseAmount = 0.85f; // MAKSYMALNE pulsowanie!
        emissionStrength = 1f; // Pełna moc
        
        Debug.Log($"   Settings: useGlow={useGlow}, pulseSpeed={pulseSpeed}, pulseAmount={pulseAmount}");
        
        // Jeśli Start() jeszcze się nie wykonał, zainicjalizuj teraz
        if (objectRenderer == null)
        {
            Debug.Log("   objectRenderer=null, calling GetComponent + ApplyGlowShader");
            objectRenderer = GetComponent<Renderer>();
            if (objectRenderer != null && useGlow)
            {
                ApplyGlowShader();
            }
            else
            {
                Debug.LogError("   ❌ Failed to get Renderer!");
            }
        }
        else if (glowMaterial != null)
        {
            Debug.Log("   Updating existing glowMaterial");
            Color hdrEmission = color * emissionStrength * 5f;
            glowMaterial.SetColor("_EmissionColor", hdrEmission);
        }
        else
        {
            Debug.LogWarning("   ⚠️ objectRenderer exists but glowMaterial is null!");
        }
    }

    // Metoda do zmiany intensywności
    public void SetGlowStrength(float strength)
    {
        emissionStrength = strength;
        if (glowMaterial != null)
        {
            glowMaterial.SetFloat("_EmissionStrength", strength);
        }
    }

    void OnDestroy()
    {
        // NIE niszcz materiału - jest współdzielony!
        // glowMaterial = null;
    }
}
