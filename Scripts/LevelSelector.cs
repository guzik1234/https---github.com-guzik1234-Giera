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
        new LevelConfig { levelName = "Easy", rows = 3, columns = 8, ballSpeed = 3f, startingLives = 5 },
        new LevelConfig { levelName = "Normal", rows = 5, columns = 10, ballSpeed = 4f, startingLives = 3 },
        new LevelConfig { levelName = "Hard", rows = 7, columns = 12, ballSpeed = 5f, startingLives = 2 },
        new LevelConfig { levelName = "Expert", rows = 8, columns = 14, ballSpeed = 6f, startingLives = 1 }
    };
    
    public static LevelConfig CurrentLevelConfig { get; private set; }

    void Start()
    {
        // Domyślnie wybierz poziom Normal
        SelectLevel(1);
    }

    public void SelectLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levels.Length)
        {
            selectedLevel = levelIndex;
            CurrentLevelConfig = levels[levelIndex];
            Debug.Log($"Selected Level: {CurrentLevelConfig.levelName}");
        }
    }

    public void LoadGameWithSelectedLevel()
    {
        if (selectedLevel >= 0 && selectedLevel < levels.Length)
        {
            CurrentLevelConfig = levels[selectedLevel];
            PlayerPrefs.SetInt("SelectedLevel", selectedLevel);
            PlayerPrefs.SetInt("LevelRows", CurrentLevelConfig.rows);
            PlayerPrefs.SetInt("LevelColumns", CurrentLevelConfig.columns);
            PlayerPrefs.SetFloat("BallSpeed", CurrentLevelConfig.ballSpeed);
            PlayerPrefs.SetInt("StartingLives", CurrentLevelConfig.startingLives);
            PlayerPrefs.Save();
            
            SceneManager.LoadScene("SampleScene");
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

    public static LevelConfig GetCurrentConfig()
    {
        if (CurrentLevelConfig == null)
        {
            // Wczytaj z PlayerPrefs lub użyj domyślnego
            return new LevelConfig 
            { 
                levelName = "Normal", 
                rows = PlayerPrefs.GetInt("LevelRows", 5), 
                columns = PlayerPrefs.GetInt("LevelColumns", 10),
                ballSpeed = PlayerPrefs.GetFloat("BallSpeed", 4f),
                startingLives = PlayerPrefs.GetInt("StartingLives", 3)
            };
        }
        return CurrentLevelConfig;
    }
}
