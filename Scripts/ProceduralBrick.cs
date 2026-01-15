using UnityEngine;

/// <summary>
/// Skrypt do tworzenia proceduralnego modelu 3D bloku
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralBrick : MonoBehaviour
{
    [Header("Brick Dimensions")]
    [SerializeField] private float width = 1f;
    [SerializeField] private float height = 0.4f;
    [SerializeField] private float depth = 0.5f;
    
    [Header("Detail")]
    [SerializeField] private bool addBevel = true;
    [SerializeField] private float bevelSize = 0.05f;
    [SerializeField] private bool addIndent = true;
    [SerializeField] private float indentDepth = 0.05f;

    void Start()
    {
        GenerateBrickMesh();
    }

    public void GenerateBrickMesh()
    {
        Mesh mesh;
        
        if (addBevel || addIndent)
        {
            mesh = GenerateDetailedBrick();
        }
        else
        {
            mesh = GenerateSimpleCube();
        }
        
        mesh.name = "ProceduralBrick";
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private Mesh GenerateSimpleCube()
    {
        Mesh mesh = new Mesh();
        
        Vector3[] vertices = new Vector3[24];
        float w = width / 2f;
        float h = height / 2f;
        float d = depth / 2f;
        
        // Front face
        vertices[0] = new Vector3(-w, -h, d);
        vertices[1] = new Vector3(w, -h, d);
        vertices[2] = new Vector3(w, h, d);
        vertices[3] = new Vector3(-w, h, d);
        
        // Back face
        vertices[4] = new Vector3(w, -h, -d);
        vertices[5] = new Vector3(-w, -h, -d);
        vertices[6] = new Vector3(-w, h, -d);
        vertices[7] = new Vector3(w, h, -d);
        
        // Top face
        vertices[8] = new Vector3(-w, h, d);
        vertices[9] = new Vector3(w, h, d);
        vertices[10] = new Vector3(w, h, -d);
        vertices[11] = new Vector3(-w, h, -d);
        
        // Bottom face
        vertices[12] = new Vector3(-w, -h, -d);
        vertices[13] = new Vector3(w, -h, -d);
        vertices[14] = new Vector3(w, -h, d);
        vertices[15] = new Vector3(-w, -h, d);
        
        // Right face
        vertices[16] = new Vector3(w, -h, d);
        vertices[17] = new Vector3(w, -h, -d);
        vertices[18] = new Vector3(w, h, -d);
        vertices[19] = new Vector3(w, h, d);
        
        // Left face
        vertices[20] = new Vector3(-w, -h, -d);
        vertices[21] = new Vector3(-w, -h, d);
        vertices[22] = new Vector3(-w, h, d);
        vertices[23] = new Vector3(-w, h, -d);
        
        int[] triangles = new int[]
        {
            // Front
            0, 2, 1, 0, 3, 2,
            // Back
            4, 6, 5, 4, 7, 6,
            // Top
            8, 10, 9, 8, 11, 10,
            // Bottom
            12, 14, 13, 12, 15, 14,
            // Right
            16, 18, 17, 16, 19, 18,
            // Left
            20, 22, 21, 20, 23, 22
        };
        
        Vector2[] uv = new Vector2[24];
        for (int i = 0; i < 6; i++)
        {
            uv[i * 4 + 0] = new Vector2(0, 0);
            uv[i * 4 + 1] = new Vector2(1, 0);
            uv[i * 4 + 2] = new Vector2(1, 1);
            uv[i * 4 + 3] = new Vector2(0, 1);
        }
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
        return mesh;
    }

    private Mesh GenerateDetailedBrick()
    {
        // Uproszczona wersja z fazą
        Mesh mesh = GenerateSimpleCube();
        
        // TODO: Można dodać bardziej zaawansowaną geometrię z fazami
        // Na razie używamy prostego cube
        
        return mesh;
    }

    void OnValidate()
    {
        if (Application.isPlaying)
        {
            GenerateBrickMesh();
        }
    }
}
