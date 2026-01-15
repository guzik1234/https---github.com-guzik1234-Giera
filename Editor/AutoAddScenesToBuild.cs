using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Linq;

/// <summary>
/// Automatycznie dodaje sceny MainMenu i SampleScene do Build Settings
/// </summary>
[InitializeOnLoad]
public class AutoAddScenesToBuild
{
    static AutoAddScenesToBuild()
    {
        // Poczekaj chwilę na start Unity
        EditorApplication.delayCall += AddScenesToBuildSettings;
    }

    [MenuItem("Tools/Add Scenes to Build Settings")]
    static void AddScenesToBuildSettings()
    {
        // Znajdź wszystkie sceny w projekcie
        string[] guids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Scenes" });
        
        if (guids.Length == 0)
        {
            Debug.LogWarning("No scenes found in Assets/Scenes folder!");
            return;
        }

        // Pobierz obecne sceny z Build Settings
        var originalScenes = EditorBuildSettings.scenes.ToList();
        bool addedAny = false;

        foreach (string guid in guids)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            
            // Sprawdź czy scena już jest w Build Settings
            bool alreadyExists = originalScenes.Any(s => s.path == scenePath);
            
            if (!alreadyExists)
            {
                // Dodaj scenę
                originalScenes.Add(new EditorBuildSettingsScene(scenePath, true));
                Debug.Log($"✓ Added to Build Settings: {sceneName}");
                addedAny = true;
            }
        }

        if (addedAny)
        {
            // Zapisz zmiany
            EditorBuildSettings.scenes = originalScenes.ToArray();
            Debug.Log($"<color=green>✓ Build Settings updated! Total scenes: {originalScenes.Count}</color>");
            
            // Pokaż Build Settings
            EditorApplication.delayCall += () => {
                EditorWindow.GetWindow(System.Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
            };
        }
        else
        {
            Debug.Log("All scenes already in Build Settings.");
        }
    }

    [MenuItem("Tools/Show Build Settings")]
    static void ShowBuildSettings()
    {
        EditorWindow.GetWindow(System.Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
    }
}
