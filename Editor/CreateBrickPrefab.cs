using UnityEngine;
using UnityEditor;

public class CreateBrickPrefab
{
    [MenuItem("Tools/Create Brick Prefab")]
    public static void CreatePrefab()
    {
        // Stwórz GameObject cegły
        GameObject brick = GameObject.CreatePrimitive(PrimitiveType.Cube);
        brick.name = "Brick";
        brick.tag = "Brick";
        brick.layer = 0;

        // Dodaj BrickController
        if (brick.GetComponent<BrickController>() == null)
            brick.AddComponent<BrickController>();

        // Dodaj Rigidbody (opcjonalnie, jeśli potrzebny)
        Rigidbody rb = brick.GetComponent<Rigidbody>();
        if (rb != null) Object.DestroyImmediate(rb);

        // Zapisz prefab do Resources/Prefabs
        string prefabPath = "Assets/Resources/Prefabs/brick.prefab";
        PrefabUtility.SaveAsPrefabAsset(brick, prefabPath);
        Object.DestroyImmediate(brick);
        Debug.Log($"✓ Brick prefab saved to {prefabPath}");
    }
}
