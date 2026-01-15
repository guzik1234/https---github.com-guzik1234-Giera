using UnityEngine;

/// <summary>
/// System particles dla efektów wizualnych - PROCEDURALNE CZĄSTECZKI
/// </summary>
public class ParticleController : MonoBehaviour
{
    public static ParticleController Instance { get; private set; }

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
        // Tworzenie proceduralnego ParticleSystem
        GameObject particleObj = new GameObject("BrickExplosion");
        particleObj.transform.position = position;
        
        ParticleSystem ps = particleObj.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startColor = new ParticleSystem.MinMaxGradient(color, color * 1.5f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.3f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(3f, 8f);
        main.startLifetime = 0.8f;
        main.maxParticles = 50;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = 1.5f; // Grawitacja
        
        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0f, 50)
        });
        
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.3f;
        
        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(color, 0f), new GradientColorKey(color * 0.5f, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;
        
        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0, 1, 1, 0));
        
        // Renderer settings - użyj Particles/Standard Unlit
        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        Material particleMat = new Material(Shader.Find("Particles/Standard Unlit") ?? Shader.Find("Sprites/Default"));
        particleMat.color = color;
        renderer.material = particleMat;
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        
        ps.Play();
        Destroy(particleObj, 2f);
    }

    public void PlayHitSparks(Vector3 position, Vector3 normal)
    {
        // Małe iskierki przy trafieniu
        GameObject sparkObj = new GameObject("HitSparks");
        sparkObj.transform.position = position;
        sparkObj.transform.rotation = Quaternion.LookRotation(normal);
        
        ParticleSystem ps = sparkObj.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startColor = new ParticleSystem.MinMaxGradient(new Color(1f, 0.4f, 0f), new Color(1f, 0.6f, 0f)); // Pomarańczowy gradient
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.15f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(2f, 5f);
        main.startLifetime = 0.3f;
        main.maxParticles = 15;
        main.gravityModifier = 0.5f;
        
        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0f, 15)
        });
        
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 30f;
        shape.radius = 0.1f;
        
        // Renderer - użyj Particle/Standard Unlit zamiast Sprites/Default
        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        Material sparkMat = new Material(Shader.Find("Particles/Standard Unlit") ?? Shader.Find("Sprites/Default"));
        sparkMat.color = new Color(1f, 0.5f, 0f); // Pomarańczowy zamiast żółtego
        if (sparkMat.HasProperty("_EmissionColor"))
        {
            sparkMat.EnableKeyword("_EMISSION");
            sparkMat.SetColor("_EmissionColor", new Color(1f, 0.5f, 0f)); // Pomarańczowy
        }
        renderer.material = sparkMat;
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        
        ps.Play();
        Destroy(sparkObj, 1f);
    }
}
