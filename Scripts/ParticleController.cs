using UnityEngine;

/// <summary>
/// System particles dla efektów wizualnych
/// </summary>
public class ParticleController : MonoBehaviour
{
    public static ParticleController Instance { get; private set; }

    [Header("Particle Prefabs")]
    [SerializeField] private ParticleSystem brickExplosionPrefab;
    [SerializeField] private ParticleSystem ballTrailPrefab;
    [SerializeField] private ParticleSystem hitSparksPrefab;
    [SerializeField] private ParticleSystem powerUpEffectPrefab;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBrickExplosion(Vector3 position, Color color)
    {
        if (brickExplosionPrefab != null)
        {
            ParticleSystem particles = Instantiate(brickExplosionPrefab, position, Quaternion.identity);
            
            // Zmień kolor particles na kolor bloku
            var main = particles.main;
            main.startColor = color;
            
            Destroy(particles.gameObject, main.duration + main.startLifetime.constantMax);
        }
    }

    public void PlayHitSparks(Vector3 position, Vector3 normal)
    {
        if (hitSparksPrefab != null)
        {
            Quaternion rotation = Quaternion.LookRotation(normal);
            ParticleSystem particles = Instantiate(hitSparksPrefab, position, rotation);
            
            var main = particles.main;
            Destroy(particles.gameObject, main.duration + main.startLifetime.constantMax);
        }
    }

    public ParticleSystem AttachTrailToBall(Transform ballTransform)
    {
        if (ballTrailPrefab != null && ballTransform != null)
        {
            ParticleSystem trail = Instantiate(ballTrailPrefab, ballTransform);
            trail.transform.localPosition = Vector3.zero;
            return trail;
        }
        return null;
    }

    public void PlayPowerUpEffect(Vector3 position)
    {
        if (powerUpEffectPrefab != null)
        {
            ParticleSystem particles = Instantiate(powerUpEffectPrefab, position, Quaternion.identity);
            
            var main = particles.main;
            Destroy(particles.gameObject, main.duration + main.startLifetime.constantMax);
        }
    }
}
