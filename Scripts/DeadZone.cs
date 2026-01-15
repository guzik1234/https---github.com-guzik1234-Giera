using UnityEngine;

/// <summary>
/// Trigger dla strefy poniżej paletki - wykrywa utratę piłki
/// </summary>
public class DeadZone : MonoBehaviour
{
    [Header("Visual Feedback")]
    [SerializeField] private bool showGizmo = true;
    [SerializeField] private Color gizmoColor = Color.red;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayLoseLife();
            }
            
            CameraController camera = FindObjectOfType<CameraController>();
            if (camera != null)
            {
                camera.Shake(0.2f);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (showGizmo)
        {
            Gizmos.color = gizmoColor;
            BoxCollider boxCollider = GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                Gizmos.DrawWireCube(transform.position + boxCollider.center, boxCollider.size);
            }
        }
    }
}
