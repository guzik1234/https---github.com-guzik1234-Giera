using UnityEngine;

/// <summary>
/// Kontroler pojedynczego bloku - obsługuje zniszczenie, punkty i efekty
/// </summary>
public class BrickController : MonoBehaviour
{
    [Header("Brick Settings")]
    [SerializeField] private int hitPoints = 1;
    [SerializeField] private int scoreValue = 10;
    [SerializeField] private Color damageColor = Color.red;
    
    [Header("Effects")]
    [SerializeField] private GameObject destroyEffectPrefab;
    [SerializeField] private float explosionForce = 500f;
    
    private int currentHitPoints;
    private Renderer meshRenderer;
    private Color originalColor;
    private AudioSource audioSource;
    private MaterialPropertyBlock propBlock;

    void Awake()
    {
        meshRenderer = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();
        currentHitPoints = hitPoints;
        originalColor = meshRenderer.material.color;
        propBlock = new MaterialPropertyBlock();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            TakeDamage();
        }
    }

    private void TakeDamage()
    {
        currentHitPoints--;

        if (currentHitPoints <= 0)
        {
            DestroyBrick();
        }
        else
        {
            // Animacja damage - zmiana koloru
            StartCoroutine(DamageFlash());
        }
    }

    private System.Collections.IEnumerator DamageFlash()
    {
        // Użycie MaterialPropertyBlock zamiast bezpośredniej modyfikacji materiału
        // Optymalizacja - nie tworzy nowych instancji materiałów
        meshRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor("_Color", damageColor);
        meshRenderer.SetPropertyBlock(propBlock);
        
        yield return new WaitForSeconds(0.1f);
        
        propBlock.SetColor("_Color", originalColor);
        meshRenderer.SetPropertyBlock(propBlock);
    }

    private void DestroyBrick()
    {
        // Odtwórz dźwięk zniszczenia
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBrickDestroy();
        }
        
        // Dodanie punktów
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
        }
        
        // Animacja zniszczenia - eksplozja fragmentów
        CreateDestroyEffect();
        
        // Usunięcie bloku z managera
        GameManager.Instance.OnBrickDestroyed(this);
        
        Destroy(gameObject);
    }

    private void CreateDestroyEffect()
    {
        // Tworzenie fragmentów (proceduralna destrukcja)
        for (int i = 0; i < 8; i++)
        {
            GameObject fragment = GameObject.CreatePrimitive(PrimitiveType.Cube);
            fragment.transform.position = transform.position;
            fragment.transform.localScale = transform.localScale * 0.3f;
            fragment.transform.rotation = Random.rotation;
            
            // Materiał z tym samym kolorem co blok
            Renderer fragRenderer = fragment.GetComponent<Renderer>();
            fragRenderer.material = new Material(meshRenderer.material);
            
            // Fizyka dla fragmentów
            Rigidbody fragRb = fragment.AddComponent<Rigidbody>();
            fragRb.mass = 0.1f;
            Vector3 explosionDir = Random.insideUnitSphere;
            fragRb.AddForce(explosionDir * explosionForce);
            fragRb.AddTorque(Random.insideUnitSphere * explosionForce);
            
            // Automatyczne usunięcie po 2 sekundach
            Destroy(fragment, 2f);
        }
    }

    // Publiczny getter dla hitpoints (do animacji)
    public float GetHealthPercentage()
    {
        return (float)currentHitPoints / hitPoints;
    }
}
