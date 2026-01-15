using UnityEngine;

/// <summary>
/// Skrypt do tworzenia procedurlanego modelu 3D paletki
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralPaddle : MonoBehaviour
{
    [Header("Paddle Dimensions")]
    [SerializeField] private float width = 2f;
    [SerializeField] private float height = 0.3f;
    [SerializeField] private float depth = 0.5f;
    [SerializeField] private int segments = 10;
    
    [Header("Style")]
    [SerializeField] private bool curved = true;
    [SerializeField] private float curvature = 0.3f;

    void Start()
    {
        GeneratePaddleMesh();
    }

    public void GeneratePaddleMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "ProceduralPaddle";
        
        // Oblicz ilość wierzchołków
        int vertexCount = (segments + 1) * 4; // 4 boki
        Vector3[] vertices = new Vector3[vertexCount];
        Vector2[] uv = new Vector2[vertexCount];
        int[] triangles = new int[segments * 24]; // 6 ścian * 2 trójkąty * 2 (góra i dół)
        
        float segmentWidth = width / segments;
        int triIndex = 0;
        
        // Generuj wierzchołki dla górnej i dolnej części
        for (int i = 0; i <= segments; i++)
        {
            float x = -width / 2f + i * segmentWidth;
            float yOffset = 0f;
            
            if (curved)
            {
                // Krzywa dla estetyki
                float normalizedX = (float)i / segments - 0.5f;
                yOffset = Mathf.Pow(normalizedX, 2) * curvature;
            }
            
            // Górna powierzchnia
            vertices[i * 4 + 0] = new Vector3(x, height / 2f + yOffset, depth / 2f);
            vertices[i * 4 + 1] = new Vector3(x, height / 2f + yOffset, -depth / 2f);
            
            // Dolna powierzchnia
            vertices[i * 4 + 2] = new Vector3(x, -height / 2f, depth / 2f);
            vertices[i * 4 + 3] = new Vector3(x, -height / 2f, -depth / 2f);
            
            // UV mapping
            float uvX = (float)i / segments;
            uv[i * 4 + 0] = new Vector2(uvX, 1);
            uv[i * 4 + 1] = new Vector2(uvX, 0);
            uv[i * 4 + 2] = new Vector2(uvX, 1);
            uv[i * 4 + 3] = new Vector2(uvX, 0);
        }
        
        // Generuj trójkąty
        for (int i = 0; i < segments; i++)
        {
            int baseIndex = i * 4;
            
            // Górna powierzchnia
            AddQuad(triangles, ref triIndex, baseIndex + 0, baseIndex + 4, baseIndex + 5, baseIndex + 1);
            
            // Dolna powierzchnia
            AddQuad(triangles, ref triIndex, baseIndex + 2, baseIndex + 3, baseIndex + 7, baseIndex + 6);
            
            // Przednia powierzchnia
            AddQuad(triangles, ref triIndex, baseIndex + 0, baseIndex + 2, baseIndex + 6, baseIndex + 4);
            
            // Tylna powierzchnia
            AddQuad(triangles, ref triIndex, baseIndex + 1, baseIndex + 5, baseIndex + 7, baseIndex + 3);
        }
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void AddQuad(int[] triangles, ref int index, int v0, int v1, int v2, int v3)
    {
        triangles[index++] = v0;
        triangles[index++] = v1;
        triangles[index++] = v2;
        
        triangles[index++] = v0;
        triangles[index++] = v2;
        triangles[index++] = v3;
    }

    void OnValidate()
    {
        // Auto-update w edytorze
        if (Application.isPlaying)
        {
            GeneratePaddleMesh();
        }
    }
}
