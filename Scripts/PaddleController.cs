using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Kontroler paletki gracza - obsługuje ruch lewoprawo i ogranicza pozycję do granic ekranu
/// </summary>
public class PaddleController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float minX = -8f;
    [SerializeField] private float maxX = 8f;
    
    [Header("Input System")]
    [SerializeField] private InputActionAsset inputActions;
    
    private InputAction moveAction;
    private Vector2 moveInput;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        // Konfiguracja Input System
        var playerMap = inputActions.FindActionMap("Player");
        moveAction = playerMap.FindAction("Move");
        
        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;
    }

    void OnEnable()
    {
        moveAction?.Enable();
    }

    void OnDisable()
    {
        moveAction?.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        // Ruch tylko w osi X
        float moveX = moveInput.x * moveSpeed;
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
