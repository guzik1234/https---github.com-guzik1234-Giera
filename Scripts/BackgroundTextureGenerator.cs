using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Generuje proceduralną teksturę tła i dodaje ją do Canvas
/// </summary>
public class BackgroundTextureGenerator : MonoBehaviour
{
    [Header("Texture Settings")]
    [SerializeField] private int textureWidth = 512;
    [SerializeField] private int textureHeight = 512;
    [SerializeField] private float noiseScale = 0.5f;
    [SerializeField] private Color baseColor = new Color(0.65f, 0.45f, 0.25f); // Jasny brąz
    [SerializeField] private Color darkColor = new Color(0.4f, 0.25f, 0.15f); // Ciemny brąz

    void Start()
    {
        CreateBackground();
    }

    private void CreateBackground()
    {
        // Generuj proceduralną teksturę
        Texture2D texture = GenerateNoiseTexture();
        
        // Znajdź lub stwórz Canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("No Canvas found - cannot add background texture");
            return;
        }

        // Stwórz Background Image jako PIERWSZE dziecko Canvas (pod wszystkim)
        GameObject bgObj = new GameObject("BackgroundTexture");
        bgObj.transform.SetParent(canvas.transform, false);
        bgObj.transform.SetAsFirstSibling(); // Pod wszystkim!

        // Dodaj Image component
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.raycastTarget = false; // NIE blokuj kliknięć!
        
        // Stwórz sprite z tekstury
        Sprite bgSprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f)
        );
        bgImage.sprite = bgSprite;

        // RectTransform - rozciągnij na cały ekran
        RectTransform rect = bgObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero; // left, bottom
        rect.offsetMax = Vector2.zero; // right, top

        Debug.Log("✓ Procedural background texture created (512x512 with wood pattern)");
    }

    private Texture2D GenerateNoiseTexture()
    {
        Texture2D texture = new Texture2D(textureWidth, textureHeight);
        texture.filterMode = FilterMode.Bilinear;

        // Generuj piksele z Perlin noise - JASNE DREWNO
        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                // Perlin noise dla tekstury drewna
                float xCoord = (float)x / textureWidth * noiseScale;
                float yCoord = (float)y / textureHeight * noiseScale;
                
                float noise = Mathf.PerlinNoise(xCoord * 15f, yCoord * 8f);
                
                // PIONOWE linie (deski drewniane)
                float woodPlanks = Mathf.Sin(xCoord * 80f) * 0.15f;
                
                // Sęki (ciemne miejsca)
                float knots = Mathf.PerlinNoise(xCoord * 30f, yCoord * 30f) * 0.3f;
                
                // Kombinacja wszystkich efektów
                float finalNoise = Mathf.Clamp01(noise + woodPlanks + knots);
                
                // Mix jasny brąz z ciemnym brązem
                Color pixelColor = Color.Lerp(darkColor, baseColor, finalNoise);
                
                texture.SetPixel(x, y, pixelColor);
            }
        }

        texture.Apply();
        return texture;
    }
}
