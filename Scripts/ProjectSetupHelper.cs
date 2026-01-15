using UnityEngine;

/// <summary>
/// Pomocniczy skrypt do setup projektu w Unity Editor
/// Dodaj do pustego GameObject i wywołaj metody z Inspector
/// </summary>
public class ProjectSetupHelper : MonoBehaviour
{
    [Header("Auto Setup")]
    [SerializeField] private bool autoSetupOnStart = false;
    
    [Header("References")]
    [SerializeField] private GameObject brickPrefab;
    [SerializeField] private Material[] brickMaterials;

    void Start()
    {
        if (autoSetupOnStart)
        {
            Debug.Log("ProjectSetupHelper: Starting auto setup...");
            // Możesz dodać automatyczną konfigurację tutaj
        }
    }

    [ContextMenu("Setup Tags")]
    public void SetupTags()
    {
        #if UNITY_EDITOR
        // Sprawdź i dodaj tagi
        string[] requiredTags = { "Paddle", "Ball", "Brick", "DeadZone", "Wall" };
        
        foreach (string tag in requiredTags)
        {
            if (!TagExists(tag))
            {
                AddTag(tag);
                Debug.Log($"Added tag: {tag}");
            }
            else
            {
                Debug.Log($"Tag already exists: {tag}");
            }
        }
        #endif
    }

    [ContextMenu("Setup Physics Materials")]
    public void SetupPhysicsMaterials()
    {
        #if UNITY_EDITOR
        // Stwórz Physics Material dla piłki
        PhysicsMaterial ballMaterial = new PhysicsMaterial("BallPhysics");
        ballMaterial.bounciness = 1.0f;
        ballMaterial.frictionCombine = PhysicsMaterialCombine.Minimum;
        ballMaterial.bounceCombine = PhysicsMaterialCombine.Maximum;
        ballMaterial.dynamicFriction = 0f;
        ballMaterial.staticFriction = 0f;
        
        // Zapisz
        UnityEditor.AssetDatabase.CreateAsset(ballMaterial, "Assets/Materials/BallPhysics.physicMaterial");
        UnityEditor.AssetDatabase.SaveAssets();
        
        Debug.Log("Created BallPhysics material in Assets/Materials/");
        #endif
    }

    [ContextMenu("Validate Scene Setup")]
    public void ValidateSceneSetup()
    {
        Debug.Log("=== Scene Validation ===");
        
        // Sprawdź kluczowe obiekty
        CheckForComponent<GameManager>("GameManager");
        CheckForComponent<CameraController>("Main Camera");
        CheckForComponent<LevelGenerator>("LevelGenerator");
        CheckForComponent<UIManager>("Canvas");
        CheckForComponent<AudioManager>("AudioManager");
        
        // Sprawdź tagi
        GameObject paddle = GameObject.FindGameObjectWithTag("Paddle");
        Debug.Log(paddle != null ? "✓ Paddle found with tag" : "✗ Paddle not found or missing tag");
        
        GameObject ball = GameObject.FindGameObjectWithTag("Ball");
        Debug.Log(ball != null ? "✓ Ball found with tag" : "✗ Ball not found or missing tag");
        
        GameObject deadZone = GameObject.FindGameObjectWithTag("DeadZone");
        Debug.Log(deadZone != null ? "✓ DeadZone found with tag" : "✗ DeadZone not found or missing tag");
        
        Debug.Log("=== Validation Complete ===");
    }

    [ContextMenu("Create Test Materials")]
    public void CreateTestMaterials()
    {
        #if UNITY_EDITOR
        Color[] colors = {
            Color.red,
            Color.blue,
            Color.green,
            Color.yellow,
            new Color(1f, 0f, 1f) // Magenta
        };
        
        string[] names = { "RedBrick", "BlueBrick", "GreenBrick", "YellowBrick", "PurpleBrick" };
        
        for (int i = 0; i < colors.Length; i++)
        {
            Material mat = new Material(Shader.Find("Custom/BrickGlowShader"));
            mat.SetColor("_Color", colors[i]);
            mat.SetColor("_EmissionColor", colors[i] * 0.5f);
            mat.SetFloat("_EmissionStrength", 1.0f);
            
            UnityEditor.AssetDatabase.CreateAsset(mat, $"Assets/Materials/{names[i]}.mat");
        }
        
        UnityEditor.AssetDatabase.SaveAssets();
        Debug.Log($"Created {colors.Length} brick materials in Assets/Materials/");
        #endif
    }

    private void CheckForComponent<T>(string objectName) where T : Component
    {
        GameObject obj = GameObject.Find(objectName);
        if (obj == null)
        {
            Debug.Log($"✗ GameObject '{objectName}' not found");
            return;
        }
        
        T component = obj.GetComponent<T>();
        if (component != null)
        {
            Debug.Log($"✓ {typeof(T).Name} found on '{objectName}'");
        }
        else
        {
            Debug.Log($"✗ {typeof(T).Name} missing on '{objectName}'");
        }
    }

    #if UNITY_EDITOR
    private bool TagExists(string tag)
    {
        try
        {
            GameObject.FindGameObjectWithTag(tag);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private void AddTag(string tag)
    {
        UnityEditor.SerializedObject tagManager = new UnityEditor.SerializedObject(
            UnityEditor.AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        
        UnityEditor.SerializedProperty tagsProp = tagManager.FindProperty("tags");
        
        // Sprawdź czy nie istnieje
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            UnityEditor.SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals(tag)) return;
        }
        
        // Dodaj nowy
        tagsProp.InsertArrayElementAtIndex(0);
        UnityEditor.SerializedProperty newTag = tagsProp.GetArrayElementAtIndex(0);
        newTag.stringValue = tag;
        tagManager.ApplyModifiedProperties();
    }
    #endif
}
