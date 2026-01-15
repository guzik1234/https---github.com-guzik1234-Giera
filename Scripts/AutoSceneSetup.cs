using UnityEngine;

/// <summary>
/// Automatyczna konfiguracja sceny - uruchamia się przy starcie i tworzy wszystkie obiekty
/// UŻYCIE: Dodaj ten skrypt do pustego GameObject w scenie i naciśnij Play
/// </summary>
public class AutoSceneSetup : MonoBehaviour
{
    [Header("Auto Setup")]
    [SerializeField] private bool autoSetup = true;
    [SerializeField] private bool setupCompleted = false;

    void Awake()
    {
        Debug.Log($"=== AutoSceneSetup.Awake() - autoSetup={autoSetup}, setupCompleted={setupCompleted} ===");
        
        if (autoSetup && !setupCompleted)
        {
            Debug.Log("=== AUTO SCENE SETUP - START ===");
            SetupCompleteScene();
            setupCompleted = true;
            Debug.Log("=== AUTO SCENE SETUP - COMPLETE ===");
            
            // Auto-start ball after 2 seconds
            Invoke(nameof(StartGame), 2f);
        }
        else
        {
            Debug.LogWarning($"AutoSceneSetup skipped: autoSetup={autoSetup}, setupCompleted={setupCompleted}");
        }
    }

    private void StartGame()
    {
        BallController ball = FindFirstObjectByType<BallController>();
        if (ball != null)
        {
            ball.LaunchBall();
            Debug.Log("🎮 GAME STARTED - Ball launched!");
        }
    }

    private void SetupCompleteScene()
    {
        // 1. Setup Camera
        SetupCamera();
        
        // 2. Setup Lighting
        SetupLighting();
        
        // 3. Create Paddle
        CreatePaddle();
        
        // 4. Create Ball
        CreateBall();
        
        // 5. Create Walls
        CreateWalls();
        
        // 6. Create DeadZone
        CreateDeadZone();
        
        // 7. Create Bricks
        CreateBricks();
        
        // 8. Create UI FIRST (before GameManager)
        UIManager createdUIManager = CreateUI();
        
        // 9. Create GameManager and assign UI
        CreateGameManager(createdUIManager);
    }

    private void SetupCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            GameObject camObj = new GameObject("Main Camera");
            mainCamera = camObj.AddComponent<Camera>();
            camObj.tag = "MainCamera";
        }

        // ZAWSZE ustaw parametry kamery (nawet jeśli już istnieje)
        mainCamera.transform.position = new Vector3(0, -3f, -10); // Jeszcze niżej - skupienie na grze
        mainCamera.orthographic = true; // MUSI być orthographic dla 2D!
        mainCamera.orthographicSize = 5f; // Balans między przybliżeniem a widokiem całej planszy
        
        if (mainCamera.GetComponent<CameraController>() == null)
        {
            mainCamera.gameObject.AddComponent<CameraController>();
        }
        
        Debug.Log($"✓ Camera setup complete (orthographic={mainCamera.orthographic}, size={mainCamera.orthographicSize})");
    }

    private void SetupLighting()
    {
        Light dirLight = FindFirstObjectByType<Light>();
        if (dirLight == null)
        {
            GameObject lightObj = new GameObject("Directional Light");
            dirLight = lightObj.AddComponent<Light>();
            dirLight.type = LightType.Directional;
        }

        dirLight.transform.rotation = Quaternion.Euler(50, -30, 0);
        dirLight.intensity = 1f;
        
        if (dirLight.GetComponent<DynamicLighting>() == null)
        {
            dirLight.gameObject.AddComponent<DynamicLighting>();
        }
        
        Debug.Log("✓ Lighting setup complete");
    }

    private void CreatePaddle()
    {
        GameObject paddle = GameObject.CreatePrimitive(PrimitiveType.Cube);
        paddle.name = "Paddle";
        paddle.tag = "Paddle";
        paddle.transform.position = new Vector3(0, -4, 0);
        paddle.transform.localScale = new Vector3(2, 0.3f, 0.5f);

        // Add components
        Rigidbody rb = paddle.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        
        // BoxCollider - ZNACZNIE większy na szerokość i wysokość dla lepszych kolizji na krawędziach
        BoxCollider paddleCollider = paddle.GetComponent<BoxCollider>();
        paddleCollider.size = new Vector3(1.3f, 2.0f, 1.2f); // Jeszcze większy collider - pełne pokrycie krawędzi
        
        // Physics Material - zapobiega przechodzeniu przez krawędzie
        PhysicsMaterial paddlePhysicsMat = new PhysicsMaterial("PaddlePhysics");
        paddlePhysicsMat.bounciness = 0f;
        paddlePhysicsMat.frictionCombine = PhysicsMaterialCombine.Minimum;
        paddlePhysicsMat.bounceCombine = PhysicsMaterialCombine.Maximum;
        paddlePhysicsMat.dynamicFriction = 0f;
        paddlePhysicsMat.staticFriction = 0f;
        paddleCollider.material = paddlePhysicsMat;

        // Use simple controller instead
        paddle.AddComponent<SimplePaddleController>();
        paddle.AddComponent<ProceduralPaddle>();

        // Material - użyj shadera który zawsze działa w buildzie
        Renderer renderer = paddle.GetComponent<Renderer>();
        Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard") ?? Shader.Find("Diffuse");
        if (shader != null)
        {
            Material mat = new Material(shader);
            mat.color = Color.cyan;
            renderer.material = mat;
        }

        Debug.Log("✓ Paddle created with SimplePaddleController");
    }

    private void CreateBall()
    {
        GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.name = "Ball";
        ball.tag = "Ball";
        ball.transform.position = new Vector3(0, -3, 0);
        ball.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

        // Rigidbody
        Rigidbody rb = ball.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // Najlepsza detekcja kolizji
        rb.mass = 1f;
        rb.interpolation = RigidbodyInterpolation.Interpolate; // Płynniejszy ruch
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;

        // SphereCollider z physics material
        SphereCollider sphereCollider = ball.GetComponent<SphereCollider>();
        sphereCollider.radius = 0.52f; // Minimalnie większy dla lepszej detekcji (domyślnie 0.5)
        PhysicsMaterial physicsMat = new PhysicsMaterial("BallPhysics");
        physicsMat.bounciness = 1.0f;
        physicsMat.frictionCombine = PhysicsMaterialCombine.Minimum;
        physicsMat.bounceCombine = PhysicsMaterialCombine.Maximum;
        physicsMat.dynamicFriction = 0f;
        physicsMat.staticFriction = 0f;
        sphereCollider.material = physicsMat;

        // Controllers
        BallController ballController = ball.AddComponent<BallController>();
        
        // TrailRenderer
        TrailRenderer trail = ball.AddComponent<TrailRenderer>();
        trail.time = 0.5f;
        trail.startWidth = 0.2f;
        trail.endWidth = 0.05f;
        trail.material = new Material(Shader.Find("Sprites/Default"));
        trail.startColor = Color.yellow;
        trail.endColor = new Color(1, 1, 0, 0);

        // Material - użyj shadera który zawsze działa w buildzie
        Renderer ballRenderer = ball.GetComponent<Renderer>();
        Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard") ?? Shader.Find("Diffuse");
        if (shader != null)
        {
            Material ballMat = new Material(shader);
            ballMat.color = Color.yellow;
            ballRenderer.material = ballMat;
        }

        Debug.Log("✓ Ball created");
    }

    private void CreateWalls()
    {
        // Left Wall - BARDZO WYSOKA i GRUBA
        GameObject leftWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftWall.name = "LeftWall";
        leftWall.tag = "Wall";
        leftWall.transform.position = new Vector3(-9.5f, 0, 0); // Dalej od gry
        leftWall.transform.localScale = new Vector3(1f, 30, 3); // Wysokość 30, grubość 3
        leftWall.AddComponent<WallController>();
        SetWallMaterial(leftWall);

        // Right Wall - BARDZO WYSOKA i GRUBA
        GameObject rightWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightWall.name = "RightWall";
        rightWall.tag = "Wall";
        rightWall.transform.position = new Vector3(9.5f, 0, 0); // Dalej od gry
        rightWall.transform.localScale = new Vector3(1f, 30, 3); // Wysokość 30, grubość 3
        rightWall.AddComponent<WallController>();
        SetWallMaterial(rightWall);

        // Top Wall - SZEROKA i GRUBA
        GameObject topWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        topWall.name = "TopWall";
        topWall.tag = "Wall";
        topWall.transform.position = new Vector3(0, 7.2f, 0); // Wyżej żeby nie było pustej przestrzeni
        topWall.transform.localScale = new Vector3(22, 1f, 3); // Szersza i grubsza
        topWall.AddComponent<WallController>();
        SetWallMaterial(topWall);

        Debug.Log("✓ Walls created (30 high, 3 thick)");
    }

    private void SetWallMaterial(GameObject wall)
    {
        Renderer renderer = wall.GetComponent<Renderer>();
        Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard") ?? Shader.Find("Diffuse");
        if (shader != null)
        {
            Material mat = new Material(shader);
            mat.color = new Color(0.3f, 0.3f, 0.3f); // Dark gray
            renderer.material = mat;
        }
    }

    private void CreateDeadZone()
    {
        GameObject deadZone = GameObject.CreatePrimitive(PrimitiveType.Cube);
        deadZone.name = "DeadZone";
        deadZone.tag = "DeadZone";
        deadZone.transform.position = new Vector3(0, -6, 0);
        deadZone.transform.localScale = new Vector3(25, 2, 5); // Większy żeby na pewno złapał

        // Make it trigger
        BoxCollider collider = deadZone.GetComponent<BoxCollider>();
        collider.isTrigger = true;

        // Remove renderer (invisible)
        Destroy(deadZone.GetComponent<Renderer>());

        deadZone.AddComponent<DeadZone>();

        Debug.Log("✓ DeadZone created (larger trigger zone)");
    }

    private void CreateBricks()
    {
        GameObject bricksContainer = new GameObject("BricksContainer");

        // Wczytaj konfigurację poziomu z LevelSelector
        var levelConfig = LevelSelector.GetCurrentConfig();
        int rows = 5;
        int columns = 10;
        Vector3 startPos = new Vector3(-5f, 5f, 0f);
        
        if (levelConfig != null)
        {
            Debug.Log($"CreateBricks using level: {levelConfig.levelName}");
            
            // Użyj konfiguracji specjalnych dla każdego poziomu
            switch (levelConfig.levelName)
            {
                case "Easy":
                    rows = 1;
                    columns = 1;
                    startPos = new Vector3(0f, 4f, 0f);
                    Debug.Log("Easy Mode: Creating 1 brick");
                    break;
                    
                case "Normal":
                    rows = 3;
                    columns = 5;
                    startPos = new Vector3(-2.5f, 4f, 0f);
                    Debug.Log($"Normal Mode: Creating {rows}x{columns} bricks");
                    break;
                    
                case "Hard":
                    rows = 5;
                    columns = 8;
                    startPos = new Vector3(-4f, 5f, 0f);
                    Debug.Log($"Hard Mode: Creating {rows}x{columns} bricks");
                    break;
                    
                case "Expert":
                    rows = 6;
                    columns = 10;
                    startPos = new Vector3(-5f, 5.5f, 0f);
                    Debug.Log($"Expert Mode: Creating {rows}x{columns} bricks");
                    break;
                    
                default:
                    Debug.LogWarning($"Unknown level '{levelConfig.levelName}' - using default 5x10");
                    break;
            }
        }
        else
        {
            Debug.LogWarning("No level config found - using default 5x10");
        }
        
        float spacing = 1.1f;

        Color[] colors = {
            Color.red,
            new Color(1f, 0.5f, 0f), // Orange
            Color.yellow,
            Color.green,
            Color.blue
        };

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                GameObject brick = GameObject.CreatePrimitive(PrimitiveType.Cube);
                brick.name = $"Brick_{row}_{col}";
                brick.tag = "Brick";
                brick.transform.position = startPos + new Vector3(col * spacing, -row * spacing, 0);
                brick.transform.localScale = new Vector3(1f, 0.4f, 0.5f);
                brick.transform.parent = bricksContainer.transform;

                // Add components
                brick.AddComponent<BrickController>();
                brick.AddComponent<ProceduralBrick>();

                // Material with color - użyj shadera który zawsze działa w buildzie
                Renderer renderer = brick.GetComponent<Renderer>();
                Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard") ?? Shader.Find("Diffuse");
                if (shader != null)
                {
                    Material mat = new Material(shader);
                    mat.color = colors[row % colors.Length];
                    if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", 0.3f);
                    if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", 0.6f);
                    renderer.material = mat;
                }
            }
        }

        Debug.Log($"✓ Created {rows * columns} bricks");
    }

    private void CreateGameManager(UIManager uiManager)
    {
        // Create AudioManager FIRST (before GameManager) to ensure it's ready
        if (AudioManager.Instance == null)
        {
            GameObject audioObj = new GameObject("AudioManager");
            audioObj.AddComponent<AudioManager>();
            Debug.Log("✓ AudioManager created (will persist between scenes)");
        }
        else
        {
            Debug.Log("✓ AudioManager already exists, reusing");
        }
        
        GameObject managerObj = new GameObject("GameManager");
        GameManager gm = managerObj.AddComponent<GameManager>();
        managerObj.AddComponent<ParticleController>();
        
        // Przypisz UIManager przez pole publiczne
        var gmType = typeof(GameManager);
        var uiManagerField = gmType.GetField("uiManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (uiManagerField != null)
        {
            uiManagerField.SetValue(gm, uiManager);
            Debug.Log("✓ GameManager created with UIManager reference");
        }
        else
        {
            Debug.LogWarning("✗ Could not assign UIManager to GameManager");
        }
    }

    private UIManager CreateUI()
    {
        // Check if TextMeshPro is available
        bool hasTMP = System.Type.GetType("TMPro.TextMeshProUGUI, Unity.TextMeshPro") != null;
        
        // Create Canvas
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        // Create Event System if not exists
        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // Create HUD Panel
        GameObject hudPanel = CreateHUDPanel(canvasObj, hasTMP);
        
        // Create Pause Panel
        GameObject pausePanel = CreatePausePanel(canvasObj, hasTMP);
        
        // Create Game Over Panel
        GameObject gameOverPanel = CreateGameOverPanel(canvasObj, hasTMP);
        
        // Create Victory Panel
        GameObject victoryPanel = CreateVictoryPanel(canvasObj, hasTMP);

        // Add UIManager and assign references
        UIManager uiManager = canvasObj.AddComponent<UIManager>();
        AssignUIReferences(uiManager, hudPanel, pausePanel, gameOverPanel, victoryPanel, hasTMP);

        // Add FixUIReferences component to auto-repair any missing references
        canvasObj.AddComponent<FixUIReferences>();

        Debug.Log("✓ Complete UI created with HUD, Pause, GameOver, and Victory screens");
        Debug.Log("✓ FixUIReferences added - will auto-repair UI connections");
        
        return uiManager; // Return UIManager so it can be assigned to GameManager
    }

    private GameObject CreateHUDPanel(GameObject canvas, bool hasTMP)
    {
        GameObject hud = new GameObject("HUD Panel");
        hud.transform.SetParent(canvas.transform, false);
        
        RectTransform hudRect = hud.AddComponent<RectTransform>();
        hudRect.anchorMin = Vector2.zero;
        hudRect.anchorMax = Vector2.one;
        hudRect.sizeDelta = Vector2.zero;
        
        // Score Text (top left)
        GameObject scoreObj = new GameObject("ScoreText");
        scoreObj.transform.SetParent(hud.transform, false);
        CreateText(scoreObj, "Score: 000000", new Vector2(200, -20), new Vector2(0, 1), new Vector2(0, 1), hasTMP, 36);
        
        // Lives Text (top right)
        GameObject livesObj = new GameObject("LivesText");
        livesObj.transform.SetParent(hud.transform, false);
        CreateText(livesObj, "Lives: 3", new Vector2(-100, -20), new Vector2(1, 1), new Vector2(1, 1), hasTMP, 36);
        
        return hud;
    }

    private GameObject CreatePausePanel(GameObject canvas, bool hasTMP)
    {
        GameObject panel = CreatePanel(canvas, "Pause Panel", new Color(0, 0, 0, 0.85f));
        
        // Title
        GameObject title = new GameObject("Title");
        title.transform.SetParent(panel.transform, false);
        CreateText(title, "PAUSED", Vector2.zero, new Vector2(0.5f, 0.7f), new Vector2(0.5f, 0.7f), hasTMP, 72);
        
        // Resume Button
        CreateButton(panel, "Resume Button", new Vector2(0.5f, 0.5f), new Vector2(300, 60), "RESUME", hasTMP, "OnResumeButton");
        
        // Main Menu Button
        CreateButton(panel, "MainMenu Button", new Vector2(0.5f, 0.35f), new Vector2(300, 60), "MAIN MENU", hasTMP, "OnMainMenuButton");
        
        panel.SetActive(false);
        return panel;
    }

    private GameObject CreateGameOverPanel(GameObject canvas, bool hasTMP)
    {
        GameObject panel = CreatePanel(canvas, "GameOver Panel", new Color(0.2f, 0, 0, 0.9f));
        
        // Title
        GameObject title = new GameObject("Title");
        title.transform.SetParent(panel.transform, false);
        CreateText(title, "GAME OVER", Vector2.zero, new Vector2(0.5f, 0.7f), new Vector2(0.5f, 0.7f), hasTMP, 72);
        
        // Final Score
        GameObject score = new GameObject("FinalScore");
        score.transform.SetParent(panel.transform, false);
        CreateText(score, "Final Score: 000000", Vector2.zero, new Vector2(0.5f, 0.55f), new Vector2(0.5f, 0.55f), hasTMP, 36);
        
        // Restart Button
        CreateButton(panel, "Restart Button", new Vector2(0.5f, 0.4f), new Vector2(300, 60), "RESTART", hasTMP, "OnRestartButton");
        
        // Main Menu Button
        CreateButton(panel, "MainMenu Button", new Vector2(0.5f, 0.25f), new Vector2(300, 60), "MAIN MENU", hasTMP, "OnMainMenuButton");
        
        panel.SetActive(false);
        return panel;
    }

    private GameObject CreateVictoryPanel(GameObject canvas, bool hasTMP)
    {
        GameObject panel = CreatePanel(canvas, "Victory Panel", new Color(0, 0.3f, 0, 0.9f));
        
        // Title
        GameObject title = new GameObject("Title");
        title.transform.SetParent(panel.transform, false);
        CreateText(title, "VICTORY!", Vector2.zero, new Vector2(0.5f, 0.7f), new Vector2(0.5f, 0.7f), hasTMP, 72);
        
        // Victory Score
        GameObject score = new GameObject("VictoryScore");
        score.transform.SetParent(panel.transform, false);
        CreateText(score, "Score: 000000", Vector2.zero, new Vector2(0.5f, 0.55f), new Vector2(0.5f, 0.55f), hasTMP, 36);
        
        // Main Menu Button
        CreateButton(panel, "MainMenu Button", new Vector2(0.5f, 0.35f), new Vector2(300, 60), "MAIN MENU", hasTMP, "OnMainMenuButton");
        
        panel.SetActive(false);
        return panel;
    }

    private GameObject CreatePanel(GameObject parent, string name, Color color)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent.transform, false);
        
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        
        UnityEngine.UI.Image img = panel.AddComponent<UnityEngine.UI.Image>();
        img.color = color;
        
        return panel;
    }

    private void CreateText(GameObject obj, string text, Vector2 anchoredPos, Vector2 anchorMin, Vector2 anchorMax, bool useTMP, int fontSize)
    {
        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = new Vector2(400, 100);
        
        if (useTMP)
        {
            var tmp = obj.AddComponent<TMPro.TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.alignment = TMPro.TextAlignmentOptions.Center;
            tmp.color = Color.white;
        }
        else
        {
            var txtComp = obj.AddComponent<UnityEngine.UI.Text>();
            txtComp.text = text;
            txtComp.fontSize = fontSize;
            txtComp.alignment = TextAnchor.MiddleCenter;
            txtComp.color = Color.white;
            txtComp.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }
    }

    private void CreateButton(GameObject parent, string name, Vector2 anchorPos, Vector2 size, string text, bool hasTMP, string functionName)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent.transform, false);
        
        RectTransform rect = btnObj.AddComponent<RectTransform>();
        rect.anchorMin = anchorPos;
        rect.anchorMax = anchorPos;
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = size;
        
        UnityEngine.UI.Image img = btnObj.AddComponent<UnityEngine.UI.Image>();
        img.color = new Color(0.2f, 0.2f, 0.2f, 0.95f);
        
        UnityEngine.UI.Button btn = btnObj.AddComponent<UnityEngine.UI.Button>();
        var colors = btn.colors;
        colors.normalColor = new Color(0.2f, 0.2f, 0.2f, 1f);
        colors.highlightedColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        colors.pressedColor = new Color(0.15f, 0.15f, 0.15f, 1f);
        btn.colors = colors;
        
        // Add listener
        btn.onClick.AddListener(() => {
            var uiManager = FindFirstObjectByType<UIManager>();
            if (uiManager != null)
            {
                uiManager.SendMessage(functionName, SendMessageOptions.DontRequireReceiver);
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
        }
        else
        {
            var txtComp = txtObj.AddComponent<UnityEngine.UI.Text>();
            txtComp.text = text;
            txtComp.fontSize = 24;
            txtComp.alignment = TextAnchor.MiddleCenter;
            txtComp.color = Color.white;
            txtComp.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }
    }

    private void AssignUIReferences(UIManager uiManager, GameObject hud, GameObject pause, GameObject gameOver, GameObject victory, bool hasTMP)
    {
        // Direct assignment (now that fields are public)
        uiManager.hudPanel = hud;
        uiManager.pausePanel = pause;
        uiManager.gameOverPanel = gameOver;
        uiManager.victoryPanel = victory;
        
        // HUD text references
        if (hasTMP)
        {
            uiManager.scoreText = hud.transform.Find("ScoreText")?.GetComponent<TMPro.TextMeshProUGUI>();
            uiManager.livesText = hud.transform.Find("LivesText")?.GetComponent<TMPro.TextMeshProUGUI>();
            uiManager.finalScoreText = gameOver.transform.Find("FinalScore")?.GetComponent<TMPro.TextMeshProUGUI>();
            uiManager.victoryScoreText = victory.transform.Find("VictoryScore")?.GetComponent<TMPro.TextMeshProUGUI>();
        }
        else
        {
            // Fallback to standard Text if TMP not available
            var scoreTextComp = hud.transform.Find("ScoreText")?.GetComponent<UnityEngine.UI.Text>();
            var livesTextComp = hud.transform.Find("LivesText")?.GetComponent<UnityEngine.UI.Text>();
            var finalScoreTextComp = gameOver.transform.Find("FinalScore")?.GetComponent<UnityEngine.UI.Text>();
            var victoryScoreTextComp = victory.transform.Find("VictoryScore")?.GetComponent<UnityEngine.UI.Text>();
            
            Debug.LogWarning("TextMeshPro not available - UI will use standard Text component");
        }
        
        Debug.Log("✓ UI Manager references assigned successfully");
    }

    [ContextMenu("Setup Scene Now")]
    public void SetupSceneNow()
    {
        setupCompleted = false;
        SetupCompleteScene();
    }

    [ContextMenu("Add Missing Tags")]
    public void AddMissingTags()
    {
        #if UNITY_EDITOR
        string[] tags = { "Paddle", "Ball", "Brick", "DeadZone", "Wall" };
        foreach (string tag in tags)
        {
            try
            {
                GameObject.FindGameObjectWithTag(tag);
            }
            catch
            {
                // Tag doesn't exist, would need to add via SerializedObject
                Debug.Log($"Tag '{tag}' needs to be added manually in Tag Manager");
            }
        }
        #endif
    }
}
