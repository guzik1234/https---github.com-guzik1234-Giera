using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Generator poziomów - tworzy układ bloków proceduralnie
/// </summary>
public class LevelGenerator : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private int rows = 5;
    [SerializeField] private int columns = 10;
    [SerializeField] private float blockSpacing = 1.1f;
    [SerializeField] private Vector3 startPosition = new Vector3(-5f, 5f, 0f);
    
    [Header("Brick Prefabs")]
    [SerializeField] private GameObject brickPrefab;
    [SerializeField] private Material[] brickMaterials;
    
    [Header("Patterns")]
    [SerializeField] private LevelPattern currentPattern = LevelPattern.Standard;
    
    [Header("Advanced Settings")]
    [SerializeField] private bool randomizeColors = true;
    [SerializeField] private bool createParentContainer = true;
    [SerializeField] private AnimationCurve hitPointsCurve;
    
    private Transform brickContainer;

    public enum LevelPattern
    {
        Standard,       // Prostokąt
        Pyramid,        // Piramida
        Diamond,        // Diament
        Checkerboard,   // Szachownica
        Random          // Losowe rozmieszczenie
    }

    void Start()
    {
        // Wczytaj konfigurację poziomu z LevelSelector
        var levelConfig = LevelSelector.GetCurrentConfig();
        
        Debug.Log("=== LEVEL GENERATOR START ===");
        Debug.Log($"PlayerPrefs LevelName: {PlayerPrefs.GetString("LevelName", "NOT SET")}");
        Debug.Log($"PlayerPrefs Rows: {PlayerPrefs.GetInt("LevelRows", -1)}");
        Debug.Log($"PlayerPrefs Columns: {PlayerPrefs.GetInt("LevelColumns", -1)}");
        
        if (levelConfig != null)
        {
            Debug.Log($"LevelConfig received: {levelConfig.levelName}");
            
            rows = levelConfig.rows;
            columns = levelConfig.columns;
            
            // SPECJALNY UKŁAD DLA KAŻDEGO POZIOMU
            switch (levelConfig.levelName)
            {
                case "Easy":
                    // Tylko 1 cegła - instant victory!
                    rows = 1;
                    columns = 1;
                    startPosition = new Vector3(0f, 4f, 0f);
                    currentPattern = LevelPattern.Standard;
                    Debug.Log("✓ Easy Mode: 1 brick - hit it to win!");
                    break;
                    
                case "Normal":
                    // 3 rzędy x 5 kolumn - średnio łatwy
                    rows = 3;
                    columns = 5;
                    startPosition = new Vector3(-2.5f, 4f, 0f);
                    currentPattern = LevelPattern.Standard;
                    Debug.Log($"✓ Normal Mode: {rows}x{columns} = {rows*columns} bricks");
                    break;
                    
                case "Hard":
                    // 5 rzędów x 8 kolumn - trudny
                    rows = 5;
                    columns = 8;
                    startPosition = new Vector3(-4f, 5f, 0f);
                    currentPattern = LevelPattern.Standard;
                    Debug.Log($"✓ Hard Mode: {rows}x{columns} = {rows*columns} bricks");
                    break;
                    
                case "Expert":
                    // 6 rzędów x 10 kolumn - bardzo trudny
                    rows = 6;
                    columns = 10;
                    startPosition = new Vector3(-5f, 5.5f, 0f);
                    currentPattern = LevelPattern.Pyramid;
                    Debug.Log($"✓ Expert Mode: {rows}x{columns} with Pyramid pattern");
                    break;
                    
                default:
                    Debug.LogWarning($"⚠ Unknown level: '{levelConfig.levelName}' - using default {rows}x{columns}");
                    break;
            }
        }
        else
        {
            Debug.LogError("✗ levelConfig is NULL!");
        }
        
        GenerateLevel();
    }

    public void GenerateLevel()
    {
        // Stwórz kontener dla bloków (organizacja hierarchii)
        if (createParentContainer)
        {
            GameObject containerObj = new GameObject("BricksContainer");
            brickContainer = containerObj.transform;
            brickContainer.position = Vector3.zero;
        }
        
        switch (currentPattern)
        {
            case LevelPattern.Standard:
                GenerateStandardPattern();
                break;
            case LevelPattern.Pyramid:
                GeneratePyramidPattern();
                break;
            case LevelPattern.Diamond:
                GenerateDiamondPattern();
                break;
            case LevelPattern.Checkerboard:
                GenerateCheckerboardPattern();
                break;
            case LevelPattern.Random:
                GenerateRandomPattern();
                break;
        }
    }

    private void GenerateStandardPattern()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                CreateBrick(row, col, row);
            }
        }
    }

    private void GeneratePyramidPattern()
    {
        for (int row = 0; row < rows; row++)
        {
            int blocksInRow = columns - row * 2;
            if (blocksInRow <= 0) break;
            
            for (int col = 0; col < blocksInRow; col++)
            {
                CreateBrick(row, col + row, row);
            }
        }
    }

    private void GenerateDiamondPattern()
    {
        int midRow = rows / 2;
        
        for (int row = 0; row < rows; row++)
        {
            int offset = Mathf.Abs(row - midRow);
            int blocksInRow = columns - offset * 2;
            
            if (blocksInRow > 0)
            {
                for (int col = 0; col < blocksInRow; col++)
                {
                    CreateBrick(row, col + offset, row);
                }
            }
        }
    }

    private void GenerateCheckerboardPattern()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                if ((row + col) % 2 == 0)
                {
                    CreateBrick(row, col, row);
                }
            }
        }
    }

    private void GenerateRandomPattern()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                if (Random.value > 0.3f) // 70% szansy na blok
                {
                    CreateBrick(row, col, row);
                }
            }
        }
    }

    private void CreateBrick(int row, int col, int colorIndex)
    {
        Vector3 position = startPosition + new Vector3(col * blockSpacing, -row * blockSpacing, 0f);
        
        GameObject brick = Instantiate(brickPrefab, position, Quaternion.identity);
        
        if (brickContainer != null)
        {
            brick.transform.SetParent(brickContainer);
        }
        
        brick.name = $"Brick_{row}_{col}";
        
        // Przypisz materiał
        Renderer renderer = brick.GetComponent<Renderer>();
        if (renderer != null && brickMaterials.Length > 0)
        {
            if (randomizeColors)
            {
                renderer.material = brickMaterials[Random.Range(0, brickMaterials.Length)];
            }
            else
            {
                int matIndex = colorIndex % brickMaterials.Length;
                renderer.material = brickMaterials[matIndex];
            }
        }
        
        // Ustaw hit points na podstawie krzywej (bloki wyżej trudniejsze)
        BrickController brickController = brick.GetComponent<BrickController>();
        if (brickController != null && hitPointsCurve != null)
        {
            float normalizedRow = (float)row / rows;
            int hitPoints = Mathf.RoundToInt(hitPointsCurve.Evaluate(normalizedRow) * 3) + 1;
            // Możemy dodać setter w BrickController lub użyć refleksji
        }
        
        // Animacja spawnu
        StartCoroutine(AnimateBrickSpawn(brick, 0.1f * (row * columns + col)));
    }

    private System.Collections.IEnumerator AnimateBrickSpawn(GameObject brick, float delay)
    {
        Vector3 originalScale = brick.transform.localScale;
        brick.transform.localScale = Vector3.zero;
        
        yield return new WaitForSeconds(delay);
        
        float duration = 0.3f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            // Elastic ease out
            float scale = Mathf.Sin(t * Mathf.PI * 0.5f);
            brick.transform.localScale = originalScale * scale;
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        brick.transform.localScale = originalScale;
    }

    public void ClearLevel()
    {
        if (brickContainer != null)
        {
            Destroy(brickContainer.gameObject);
        }
        else
        {
            BrickController[] bricks = FindObjectsByType<BrickController>(FindObjectsSortMode.None);
            foreach (var brick in bricks)
            {
                Destroy(brick.gameObject);
            }
        }
    }

    public void RegenerateLevel()
    {
        ClearLevel();
        GenerateLevel();
    }
}
