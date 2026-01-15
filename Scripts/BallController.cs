using UnityEngine;

/// <summary>
/// Kontroler piłki - obsługuje ruch, odbicia i efekty wizualne
/// </summary>
public class BallController : MonoBehaviour
{
    [Header("Ball Settings")]
    [SerializeField] private float initialSpeed = 4f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float speedIncrement = 0.3f;
    
    [Header("Start Settings")]
    [SerializeField] private bool startOnAwake = false;
    [SerializeField] private Vector3 startDirection = new Vector3(0.7f, 1f, 0f);
    
    private Rigidbody rb;
    private TrailRenderer trail;
    private bool isMoving = false;
    private float currentSpeed;
    private AudioSource audioSource;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();
        audioSource = GetComponent<AudioSource>();
        
        // Wczytaj prędkość z konfiguracji poziomu
        var levelConfig = LevelSelector.GetCurrentConfig();
        if (levelConfig != null)
        {
            initialSpeed = levelConfig.ballSpeed;
            maxSpeed = initialSpeed * 2.5f;
            Debug.Log($"Ball speed set to: {initialSpeed}");
        }
        
        currentSpeed = initialSpeed;
    }

    void Start()
    {
        if (startOnAwake)
        {
            LaunchBall();
        }
    }

    public void LaunchBall()
    {
        if (!isMoving)
        {
            isMoving = true;
            Vector3 direction = startDirection.normalized;
            rb.linearVelocity = direction * currentSpeed;
        }
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            // Utrzymywanie stałej prędkości
            Vector3 currentVel = rb.linearVelocity;
            rb.linearVelocity = currentVel.normalized * currentSpeed;
            
            // Zapobieganie ruchu pionowemu lub poziomemu (stuck)
            Vector3 vel = rb.linearVelocity;
            if (Mathf.Abs(vel.x) < 0.5f)
            {
                vel.x = vel.x > 0 ? 0.5f : -0.5f;
            }
            if (Mathf.Abs(vel.y) < 0.5f)
            {
                vel.y = vel.y > 0 ? 0.5f : -0.5f;
            }
            rb.linearVelocity = vel.normalized * currentSpeed;
            
            // WYMUSZENIE Z=0 - piłka MUSI być w płaszczyźnie gry!
            Vector3 pos = transform.position;
            if (Mathf.Abs(pos.z) > 0.01f)
            {
                pos.z = 0f;
                transform.position = pos;
                Debug.LogWarning($"Ball Z corrected: was {transform.position.z}, now 0");
            }
            
            // Rotacja piłki (animacja proceduralna)
            transform.Rotate(Vector3.forward, rb.linearVelocity.magnitude * 2f);
            
            // Sprawdź czy piłka nie spadła za nisko (failsafe)
            if (transform.position.y < -7f)
            {
                Debug.Log("Ball fell too low - Auto Reset!");
                ResetBall();
                Invoke(nameof(LaunchBall), 1f);
                return;
            }
            
            // Zapobieganie wyjściu poza planszę (użyj istniejącej zmiennej pos)
            pos = transform.position;
            if (pos.x < -9f || pos.x > 9f || pos.y > 11f)
            {
                // Odbicie od granicy
                if (pos.x < -9f) { vel.x = Mathf.Abs(vel.x); pos.x = -9f; }
                if (pos.x > 9f) { vel.x = -Mathf.Abs(vel.x); pos.x = 9f; }
                if (pos.y > 11f) { vel.y = -Mathf.Abs(vel.y); pos.y = 11f; }
                transform.position = pos;
                rb.linearVelocity = vel.normalized * currentSpeed;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Specjalne odbicie od paletki
        if (collision.gameObject.CompareTag("Paddle"))
        {
            // Dźwięk odbicia od paletki
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayBallHitPaddle();
            }
            
            // Try both controllers
            PaddleController paddle = collision.gameObject.GetComponent<PaddleController>();
            SimplePaddleController simplePaddle = collision.gameObject.GetComponent<SimplePaddleController>();
            
            if (paddle != null)
            {
                paddle.OnBallHit();
            }
            else if (simplePaddle != null)
            {
                simplePaddle.OnBallHit();
            }

            // Modyfikacja kąta odbicia w zależności od miejsca uderzenia
            // Oblicz punkt uderzenia względem centrum paletki (-0.5 do 0.5)
            float hitPoint = (transform.position.x - collision.transform.position.x) / collision.collider.bounds.size.x;
            hitPoint = Mathf.Clamp(hitPoint, -0.9f, 0.9f); // Szerszy zakres dla skrajnych odbić
            
            // Zwiększ kąt dla lepszych odbić na krawędziach
            float angle = hitPoint * 75f; // Maksymalnie 75 stopni w obie strony
            Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.up;
            
            // Ustaw nową prędkość
            rb.linearVelocity = direction * currentSpeed;
            
            // Wymuszenie ruchu w górę (bezpieczeństwo)
            if (rb.linearVelocity.y < 0.5f)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, 0.5f), 0).normalized * currentSpeed;
            }
            
            // Dodatkowe zabezpieczenie dla skrajnych krawędzi - odbij zawsze w górę
            Vector3 contactPoint = collision.contacts[0].point;
            float paddleHalfWidth = collision.collider.bounds.size.x / 2f;
            float distanceFromCenter = Mathf.Abs(contactPoint.x - collision.transform.position.x);
            
            // Jeśli uderzenie było bardzo blisko krawędzi
            if (distanceFromCenter > paddleHalfWidth * 0.7f)
            {
                // Wymuszenie mocnego odbicia w górę
                Vector3 vel = rb.linearVelocity;
                vel.y = Mathf.Max(Mathf.Abs(vel.y), currentSpeed * 0.7f);
                rb.linearVelocity = vel.normalized * currentSpeed;
            }
        }
        
        // Odbicie od ścian - dodaj losowy kąt aby uniknąć zapętlenia
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.name.Contains("Wall"))
        {
            // Dźwięk odbicia od ściany
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayBallHitWall();
            }
            
            Vector3 vel = rb.linearVelocity;
            vel += new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0);
            rb.linearVelocity = vel.normalized * currentSpeed;
        }
        
        // Odbicie od bloku
        if (collision.gameObject.CompareTag("Brick"))
        {
            // Dźwięk odbicia od bloku
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayBallHitBrick();
            }
        }
        
        // Zwiększenie prędkości przy każdym uderzeniu
        if (currentSpeed < maxSpeed)
        {
            currentSpeed += speedIncrement;
        }

        // Efekt particles przy uderzeniu
        CreateHitEffect(collision.contacts[0].point);
    }

    private void CreateHitEffect(Vector3 position)
    {
        // Trail efekt jest już włączony przez TrailRenderer
        // Można dodać particles tutaj
    }

    public void ResetBall()
    {
        isMoving = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = new Vector3(0f, -3f, 0f);
        transform.rotation = Quaternion.identity;
        currentSpeed = initialSpeed;
        
        if (trail != null)
        {
            trail.Clear();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Sprawdzenie czy piłka spadła poniżej
        if (other.CompareTag("DeadZone"))
        {
            Debug.Log("Ball hit DeadZone - Resetting!");
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnBallLost();
            }
            else
            {
                // Fallback - auto reset jeśli nie ma GameManagera
                ResetBall();
                Invoke(nameof(LaunchBall), 1f);
            }
        }
    }
}
