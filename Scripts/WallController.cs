using UnityEngine;

/// <summary>
/// Skrypt dla ścian - odbicie piłki i efekty wizualne
/// </summary>
public class WallController : MonoBehaviour
{
    [Header("Effects")]
    [SerializeField] private ParticleSystem hitParticles;
    [SerializeField] private float particleLifetime = 0.5f;
    
    private Renderer meshRenderer;
    private Color originalColor;

    void Awake()
    {
        meshRenderer = GetComponent<Renderer>();
        if (meshRenderer != null)
        {
            originalColor = meshRenderer.material.color;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            // Particles w miejscu kontaktu
            if (hitParticles != null && collision.contacts.Length > 0)
            {
                Vector3 hitPoint = collision.contacts[0].point;
                ParticleSystem particles = Instantiate(hitParticles, hitPoint, Quaternion.identity);
                Destroy(particles.gameObject, particleLifetime);
            }
            
            // Dźwięk
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayBallHitWall();
            }
            
            // Screen shake
            CameraController camera = FindFirstObjectByType<CameraController>();
            if (camera != null)
            {
                camera.Shake(0.05f, 0.08f);
            }
        }
    }
}
