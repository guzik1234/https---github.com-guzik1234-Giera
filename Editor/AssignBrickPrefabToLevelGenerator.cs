using UnityEngine;
using UnityEditor;

public class AssignBrickPrefabToLevelGenerator : EditorWindow
{
    [MenuItem("Tools/Assign Brick Prefab To LevelGenerator")]
    public static void AssignPrefab()
    {
        // Znajdź prefab cegły
        var brickPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Prefabs/brick.prefab");
        if (brickPrefab == null)
        {
            Debug.LogError("Nie znaleziono brick.prefab w Assets/Resources/Prefabs/");
            return;
        }

        // Znajdź LevelGenerator na scenie
        var levelGen = Object.FindObjectOfType<LevelGenerator>();
        if (levelGen == null)
        {
            Debug.LogError("Nie znaleziono LevelGenerator na scenie!");
            return;
        }

        // Przypisz prefab
        Undo.RecordObject(levelGen, "Assign Brick Prefab");
        var so = new SerializedObject(levelGen);
        var prop = so.FindProperty("brickPrefab");
        prop.objectReferenceValue = brickPrefab;
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(levelGen);
        Debug.Log("✓ Przypisano brick.prefab do LevelGenerator.brickPrefab!");
    }
}
