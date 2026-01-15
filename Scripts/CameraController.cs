using UnityEngine;

/// <summary>
/// Kontroler kamery - obsługuje efekty screen shake i smooth follow
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private bool isOrthographic = true;
    [SerializeField] private float orthographicSize = 10f;
    [SerializeField] private float fieldOfView = 60f;
    
    [Header("Follow Settings")]
    [SerializeField] private Transform followTarget;
    [SerializeField] private bool smoothFollow = false;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);
    
    [Header("Screen Shake")]
    [SerializeField] private float shakeDecay = 0.002f;
    
    private Vector3 originalPosition;
    private float shakeIntensity = 0f;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = GetComponent<Camera>();
        }
        
        // Konfiguracja kamery
        mainCamera.orthographic = isOrthographic;
        
        if (isOrthographic)
        {
            mainCamera.orthographicSize = orthographicSize;
        }
        else
        {
            mainCamera.fieldOfView = fieldOfView;
        }
        
        originalPosition = transform.position;
    }

    void LateUpdate()
    {
        // Smooth follow target
        if (followTarget != null && smoothFollow)
        {
            Vector3 desiredPosition = followTarget.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
            originalPosition = smoothedPosition;
        }
        
        // Screen shake effect - DISABLED
        // Można włączyć zmieniając false na true
        if (false && shakeIntensity > 0)
        {
            transform.position = originalPosition + Random.insideUnitSphere * shakeIntensity;
            shakeIntensity -= shakeDecay * Time.deltaTime;
            shakeIntensity = Mathf.Max(0f, shakeIntensity);
        }
        else
        {
            transform.position = originalPosition;
        }
    }

    /// <summary>
    /// Wywołuje efekt wstrząsu kamery
    /// </summary>
    public void Shake(float intensity)
    {
        shakeIntensity = intensity;
    }

    /// <summary>
    /// Animacja zoom in/out
    /// </summary>
    public void AnimateZoom(float targetSize, float duration)
    {
        if (isOrthographic)
        {
            StartCoroutine(ZoomCoroutine(targetSize, duration));
        }
    }

    private System.Collections.IEnumerator ZoomCoroutine(float targetSize, float duration)
    {
        float startSize = mainCamera.orthographicSize;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            mainCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        mainCamera.orthographicSize = targetSize;
    }

    /// <summary>
    /// Ustawia cel dla kamery
    /// </summary>
    public void SetFollowTarget(Transform target, bool smooth = false)
    {
        followTarget = target;
        smoothFollow = smooth;
    }
}
