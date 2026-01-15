using UnityEngine;

/// <summary>
/// Skrypt dla ścian - odbicie piłki i efekty wizualne
/// </summary>
public class WallController : MonoBehaviour
{
    [Header("Effects")]
    [SerializeField] private ParticleSystem hitParticles;
    [SerializeField] private float particleLifetime = 0.5f;
    
    [Header("Visual Feedback")]
    [SerializeField] private bool flashOnHit = true;
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.1f;
    
    private Renderer meshRenderer;
    private Color originalColor;
    private MaterialPropertyBlock propBlock;

    void Awake()
    {
        meshRenderer = GetComponent<Renderer>();
        if (meshRenderer != null)
        {
            originalColor = meshRenderer.material.color;
            propBlock = new MaterialPropertyBlock();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            // Efekty wizualne
            if (flashOnHit && meshRenderer != null)
            {
                StartCoroutine(FlashEffect());
            }
            
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
                camera.Shake(0.05f);
            }
        }
    }

    private System.Collections.IEnumerator FlashEffect()
    {
        meshRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor("_EmissionColor", flashColor);
        meshRenderer.SetPropertyBlock(propBlock);
        
        yield return new WaitForSeconds(flashDuration);
        
        propBlock.SetColor("_EmissionColor", Color.black);
        meshRenderer.SetPropertyBlock(propBlock);
    }
}
