using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// Prosty kontroler paletki - działa ze starym i nowym Input System
/// </summary>
public class SimplePaddleController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float minX = -8f;
    [SerializeField] private float maxX = 8f;
    
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float moveInput = 0f;
        
        // Kompatybilność ze starym i nowym Input System
        #if ENABLE_INPUT_SYSTEM
        // Nowy Input System
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            {
                moveInput = -1f;
            }
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            {
                moveInput = 1f;
            }
        }
        #else
        // Stary Input System (fallback)
        moveInput = Input.GetAxisRaw("Horizontal");
        #endif
        
        // Ruch tylko w osi X
        float moveX = moveInput * moveSpeed;
        Vector3 newPosition = transform.position + new Vector3(moveX * Time.fixedDeltaTime, 0, 0);
        
        // Ograniczenie ruchu do granic
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        
        rb.MovePosition(newPosition);
    }

    // Animacja proceduralna - squeeze effect przy uderzeniu
    public void OnBallHit()
    {
        StartCoroutine(SqueezeEffect());
    }

    private System.Collections.IEnumerator SqueezeEffect()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 squeezeScale = new Vector3(originalScale.x * 1.2f, originalScale.y * 0.8f, originalScale.z);
        
        float duration = 0.1f;
        float elapsed = 0f;
        
        // Ściśnięcie
        while (elapsed < duration)
        {
            transform.localScale = Vector3.Lerp(originalScale, squeezeScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        elapsed = 0f;
        // Powrót do normy
        while (elapsed < duration)
        {
            transform.localScale = Vector3.Lerp(squeezeScale, originalScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        transform.localScale = originalScale;
    }
}
