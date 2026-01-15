using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// System wyboru poziomów trudności
/// </summary>
public class LevelSelector : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private int selectedLevel = 1;
    
    // Level Configuration Class
    [System.Serializable]
    public class LevelConfig
    {
        public string levelName;
        public int rows = 5;
        public int columns = 10;
        public float ballSpeed = 4f;
        public int startingLives = 3;
    }
    
    [Header("Level Configurations")]
    [SerializeField] private LevelConfig[] levels = new LevelConfig[]
    {
        new LevelConfig { levelName = "Easy", rows = 1, columns = 1, ballSpeed = 3f, startingLives = 5 },
        new LevelConfig { levelName = "Normal", rows = 3, columns = 5, ballSpeed = 4f, startingLives = 3 },
        new LevelConfig { levelName = "Hard", rows = 5, columns = 8, ballSpeed = 5f, startingLives = 2 },
        new LevelConfig { levelName = "Expert", rows = 6, columns = 10, ballSpeed = 6f, startingLives = 1 }
    };
    
    public static LevelConfig CurrentLevelConfig { get; private set; }

    void Start()
    {
        // Wyczyść statyczną zmienną żeby zawsze wczytywać świeże dane
        CurrentLevelConfig = null;
        
        Debug.Log($"LevelSelector.Start() - PlayerPrefs LevelName: {PlayerPrefs.GetString("LevelName", "NOT SET")}");
        
        // NIE ustawiaj domyślnego poziomu - zawsze wczytuj z PlayerPrefs
        // (ustawienie poziomu jest w MainMenu przez przyciski)
    }

    public void SelectLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levels.Length)
        {
            selectedLevel = levelIndex;
            CurrentLevelConfig = levels[levelIndex];
            
            // ZAPISZ NATYCHMIAST do PlayerPrefs
            PlayerPrefs.SetString("LevelName", CurrentLevelConfig.levelName);
            PlayerPrefs.SetInt("SelectedLevel", selectedLevel);
            PlayerPrefs.SetInt("LevelRows", CurrentLevelConfig.rows);
            PlayerPrefs.SetInt("LevelColumns", CurrentLevelConfig.columns);
            PlayerPrefs.SetFloat("BallSpeed", CurrentLevelConfig.ballSpeed);
            PlayerPrefs.SetInt("StartingLives", CurrentLevelConfig.startingLives);
            PlayerPrefs.Save();
            
            Debug.Log($"✓ Selected Level: {CurrentLevelConfig.levelName} (saved to PlayerPrefs)");
        }
        else
        {
            Debug.LogError($"Invalid level index: {levelIndex}");
        }
    }

    public void LoadGameWithSelectedLevel()
    {
        if (selectedLevel >= 0 && selectedLevel < levels.Length)
        {
            // Dane już zapisane przez SelectLevel(), tylko załaduj scenę
            Debug.Log($"🎮 Loading game with level: {CurrentLevelConfig.levelName}");
            Debug.Log($"   Settings: {CurrentLevelConfig.rows}x{CurrentLevelConfig.columns}, {CurrentLevelConfig.startingLives} lives, speed {CurrentLevelConfig.ballSpeed}");
            
            // Spróbuj załadować scenę
            try
            {
                SceneManager.LoadScene("SampleScene");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load SampleScene: {e.Message}");
                Debug.LogError("Make sure SampleScene is in Build Settings!");
                Debug.LogError("Go to: File → Build Settings → Add Open Scenes");
            }
        }
        else
        {
            Debug.LogError($"Invalid level index: {selectedLevel}");
        }
    }

    public void OnEasyButton() => SelectLevel(0);
    public void OnNormalButton() => SelectLevel(1);
    public void OnHardButton() => SelectLevel(2);
    public void OnExpertButton() => SelectLevel(3);
    
    public void OnPlayButton()
    {
        LoadGameWithSelectedLevel();
    }

    public void OnBackButton()
    {
        // Powrót do głównego menu
        gameObject.SetActive(false);
    }
    
    public void OnNextLevelButton()
    {
        // Przełącz na następny poziom
        int currentIndex = PlayerPrefs.GetInt("SelectedLevel", 0);
        int nextIndex = currentIndex + 1;
        
        // Jeśli to był ostatni poziom, wróć do Easy
        if (nextIndex >= levels.Length)
        {
            nextIndex = 0;
            Debug.Log("✅ Completed all levels! Starting from Easy again.");
        }
        
        Debug.Log($"🎮 Next Level: {levels[currentIndex].levelName} -> {levels[nextIndex].levelName}");
        
        SelectLevel(nextIndex);
        LoadGameWithSelectedLevel();
    }

    public static LevelConfig GetCurrentConfig()
    {
        Debug.Log("=== GetCurrentConfig() called ===");
        
        if (CurrentLevelConfig == null)
        {
            // Wczytaj z PlayerPrefs
            string savedLevelName = PlayerPrefs.GetString("LevelName", "NOT_SET");
            int savedRows = PlayerPrefs.GetInt("LevelRows", -1);
            int savedColumns = PlayerPrefs.GetInt("LevelColumns", -1);
            float savedSpeed = PlayerPrefs.GetFloat("BallSpeed", -1f);
            int savedLives = PlayerPrefs.GetInt("StartingLives", -1);
            
            Debug.Log($"Reading from PlayerPrefs:");
            Debug.Log($"  LevelName: {savedLevelName}");
            Debug.Log($"  Rows: {savedRows}, Columns: {savedColumns}");
            Debug.Log($"  Speed: {savedSpeed}, Lives: {savedLives}");
            
            // Jeśli nie ma zapisanych danych, użyj Normal jako domyślnego
            if (savedLevelName == "NOT_SET")
            {
                Debug.LogWarning("No saved level found - using Normal as default");
                savedLevelName = "Normal";
                savedRows = 3;
                savedColumns = 5;
                savedSpeed = 4f;
                savedLives = 3;
            }
            
            var config = new LevelConfig 
            { 
                levelName = savedLevelName,
                rows = savedRows, 
                columns = savedColumns,
                ballSpeed = savedSpeed,
                startingLives = savedLives
            };
            
            Debug.Log($"Returning config: {config.levelName} ({config.rows}x{config.columns})");
            return config;
        }
        
        Debug.Log($"Returning cached config: {CurrentLevelConfig.levelName}");
        return CurrentLevelConfig;
    }
}
