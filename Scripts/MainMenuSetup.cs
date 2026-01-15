using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Automatyczna konfiguracja sceny Main Menu
/// </summary>
public class MainMenuSetup : MonoBehaviour
{
    [Header("Auto Setup")]
    [SerializeField] private bool autoSetup = true;

    void Start()
    {
        if (autoSetup)
        {
            SetupMainMenu();
        }
    }

    private void SetupMainMenu()
    {
        // Check if TextMeshPro is available
        bool hasTMP = System.Type.GetType("TMPro.TextMeshProUGUI, Unity.TextMeshPro") != null;
        
        // Setup Camera
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            GameObject camObj = new GameObject("Main Camera");
            mainCamera = camObj.AddComponent<Camera>();
            camObj.tag = "MainCamera";
        }
        mainCamera.transform.position = new Vector3(0, 0, -10);
        mainCamera.backgroundColor = new Color(0.1f, 0.1f, 0.2f);

        // Create Canvas
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();

        // Create Event System
        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // Main Panel
        GameObject mainPanel = CreateMainPanel(canvasObj, hasTMP);
        
        // Level Select Panel
        GameObject levelPanel = CreateLevelSelectPanel(canvasObj, hasTMP);
        levelPanel.SetActive(false);

        // Add MainMenuManager
        GameObject managerObj = new GameObject("MainMenuManager");
        MainMenuManager manager = managerObj.AddComponent<MainMenuManager>();
        AssignMainMenuReferences(manager, mainPanel, levelPanel);

        // Add LevelSelector to levelPanel
        LevelSelector levelSelector = levelPanel.AddComponent<LevelSelector>();

        Debug.Log("✓ Main Menu setup complete");
    }

    private GameObject CreateMainPanel(GameObject canvas, bool hasTMP)
    {
        GameObject panel = new GameObject("Main Panel");
        panel.transform.SetParent(canvas.transform, false);
        
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        
        // Background
        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.1f, 0.2f, 1f);

        // Title
        GameObject title = new GameObject("Title");
        title.transform.SetParent(panel.transform, false);
        CreateText(title, "ARKANOID 3D", Vector2.zero, new Vector2(0.5f, 0.75f), new Vector2(0.5f, 0.75f), hasTMP, 84);

        // Buttons
        CreateButton(panel, "Play Button", new Vector2(0.5f, 0.55f), new Vector2(350, 70), "PLAY", hasTMP, () => {
            panel.SetActive(false);
            panel.transform.parent.Find("Level Select Panel")?.gameObject.SetActive(true);
        });
        
        CreateButton(panel, "Quit Button", new Vector2(0.5f, 0.4f), new Vector2(350, 70), "QUIT", hasTMP, () => {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        });

        return panel;
    }

    private GameObject CreateLevelSelectPanel(GameObject canvas, bool hasTMP)
    {
        GameObject panel = new GameObject("Level Select Panel");
        panel.transform.SetParent(canvas.transform, false);
        
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        
        // Background
        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.1f, 0.2f, 1f);

        // Title
        GameObject title = new GameObject("Title");
        title.transform.SetParent(panel.transform, false);
        CreateText(title, "SELECT DIFFICULTY", Vector2.zero, new Vector2(0.5f, 0.8f), new Vector2(0.5f, 0.8f), hasTMP, 64);

        // Level buttons
        float startY = 0.6f;
        float spacing = 0.12f;
        
        CreateLevelButton(panel, "Easy Button", new Vector2(0.5f, startY), new Vector2(400, 70), 
            "EASY - 1 Brick | 5 Lives", hasTMP, new Color(0.2f, 0.6f, 0.2f), 0);
        
        CreateLevelButton(panel, "Normal Button", new Vector2(0.5f, startY - spacing), new Vector2(400, 70), 
            "NORMAL - 15 Bricks | 3 Lives", hasTMP, new Color(0.3f, 0.3f, 0.6f), 1);
        
        CreateLevelButton(panel, "Hard Button", new Vector2(0.5f, startY - spacing * 2), new Vector2(400, 70), 
            "HARD - 40 Bricks | 2 Lives", hasTMP, new Color(0.6f, 0.4f, 0.2f), 2);
        
        CreateLevelButton(panel, "Expert Button", new Vector2(0.5f, startY - spacing * 3), new Vector2(400, 70), 
            "EXPERT - 60 Bricks | 1 Life", hasTMP, new Color(0.7f, 0.2f, 0.2f), 3);

        // Back button
        CreateButton(panel, "Back Button", new Vector2(0.5f, 0.15f), new Vector2(250, 60), "BACK", hasTMP, () => {
            panel.SetActive(false);
            panel.transform.parent.Find("Main Panel")?.gameObject.SetActive(true);
        });

        return panel;
    }

    private void CreateText(GameObject obj, string text, Vector2 anchoredPos, Vector2 anchorMin, Vector2 anchorMax, bool useTMP, int fontSize)
    {
        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = new Vector2(800, 150);
        
        if (useTMP)
        {
            var tmp = obj.AddComponent<TMPro.TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.alignment = TMPro.TextAlignmentOptions.Center;
            tmp.color = Color.white;
            tmp.fontStyle = TMPro.FontStyles.Bold;
        }
        else
        {
            var txtComp = obj.AddComponent<Text>();
            txtComp.text = text;
            txtComp.fontSize = fontSize;
            txtComp.alignment = TextAnchor.MiddleCenter;
            txtComp.color = Color.white;
            txtComp.fontStyle = FontStyle.Bold;
            txtComp.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }
    }

    private void CreateButton(GameObject parent, string name, Vector2 anchorPos, Vector2 size, string text, bool hasTMP, System.Action onClick)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent.transform, false);
        
        RectTransform rect = btnObj.AddComponent<RectTransform>();
        rect.anchorMin = anchorPos;
        rect.anchorMax = anchorPos;
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = size;
        
        Image img = btnObj.AddComponent<Image>();
        img.color = new Color(0.25f, 0.25f, 0.35f, 1f);
        
        Button btn = btnObj.AddComponent<Button>();
        var colors = btn.colors;
        colors.normalColor = new Color(0.25f, 0.25f, 0.35f, 1f);
        colors.highlightedColor = new Color(0.35f, 0.35f, 0.45f, 1f);
        colors.pressedColor = new Color(0.2f, 0.2f, 0.3f, 1f);
        btn.colors = colors;
        
        if (onClick != null)
            btn.onClick.AddListener(() => onClick());
        
        // Button Text
        GameObject txtObj = new GameObject("Text");
        txtObj.transform.SetParent(btnObj.transform, false);
        
        RectTransform txtRect = txtObj.AddComponent<RectTransform>();
        txtRect.anchorMin = Vector2.zero;
        txtRect.anchorMax = Vector2.one;
        txtRect.sizeDelta = Vector2.zero;
        
        if (hasTMP)
        {
            var tmp = txtObj.AddComponent<TMPro.TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 32;
            tmp.alignment = TMPro.TextAlignmentOptions.Center;
            tmp.color = Color.white;
            tmp.fontStyle = TMPro.FontStyles.Bold;
        }
        else
        {
            var txtComp = txtObj.AddComponent<Text>();
            txtComp.text = text;
            txtComp.fontSize = 28;
            txtComp.alignment = TextAnchor.MiddleCenter;
            txtComp.color = Color.white;
            txtComp.fontStyle = FontStyle.Bold;
            txtComp.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }
    }

    private void CreateLevelButton(GameObject parent, string name, Vector2 anchorPos, Vector2 size, string text, bool hasTMP, Color color, int levelIndex)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent.transform, false);
        
        RectTransform rect = btnObj.AddComponent<RectTransform>();
        rect.anchorMin = anchorPos;
        rect.anchorMax = anchorPos;
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = size;
        
        Image img = btnObj.AddComponent<Image>();
        img.color = color;
        
        Button btn = btnObj.AddComponent<Button>();
        var colors = btn.colors;
        colors.normalColor = color;
        colors.highlightedColor = color * 1.2f;
        colors.pressedColor = color * 0.8f;
        btn.colors = colors;
        
        btn.onClick.AddListener(() => {
            Debug.Log($"🔴 BUTTON CLICKED: levelIndex={levelIndex}");
            var levelSelector = parent.GetComponent<LevelSelector>();
            if (levelSelector != null)
            {
                Debug.Log($"🟢 LevelSelector found, calling SelectLevel({levelIndex})");
                levelSelector.SelectLevel(levelIndex);
                levelSelector.OnPlayButton();
            }
            else
            {
                Debug.LogError("🔴 LevelSelector NOT FOUND on parent!");
            }
        });
        
        // Button Text
        GameObject txtObj = new GameObject("Text");
        txtObj.transform.SetParent(btnObj.transform, false);
        
        RectTransform txtRect = txtObj.AddComponent<RectTransform>();
        txtRect.anchorMin = Vector2.zero;
        txtRect.anchorMax = Vector2.one;
        txtRect.sizeDelta = Vector2.zero;
        
        if (hasTMP)
        {
            var tmp = txtObj.AddComponent<TMPro.TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 28;
            tmp.alignment = TMPro.TextAlignmentOptions.Center;
            tmp.color = Color.white;
            tmp.fontStyle = TMPro.FontStyles.Bold;
        }
        else
        {
            var txtComp = txtObj.AddComponent<Text>();
            txtComp.text = text;
            txtComp.fontSize = 24;
            txtComp.alignment = TextAnchor.MiddleCenter;
            txtComp.color = Color.white;
            txtComp.fontStyle = FontStyle.Bold;
            txtComp.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }
    }

    private void AssignMainMenuReferences(MainMenuManager manager, GameObject mainPanel, GameObject levelPanel)
    {
        // Direct assignment (now that fields are public)
        manager.mainPanel = mainPanel;
        manager.optionsPanel = levelPanel;
        Debug.Log("✓ Main Menu Manager references assigned");
    }
}
