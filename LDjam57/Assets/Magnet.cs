using UnityEngine;

public class Magnet : MonoBehaviour
{
    [Header("Splash Settings")]
    [SerializeField] private GameObject splashEffectPrefab;
    [SerializeField] private Transform splashParent; // optional, for organizing in hierarchy
    [SerializeField] private Vector3 splashOffset = Vector3.zero;

    private bool hasSplashed = false;

    private void OnEnable()
    {
        hasSplashed = false; // Reset when magnet is reused
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasSplashed) return;

        if (other.CompareTag("WaterSurface"))
        {
            hasSplashed = true;
            Vector3 splashPosition = new Vector3(transform.position.x, other.transform.position.y, 0f) + splashOffset;
            
            if (splashEffectPrefab != null)
                Instantiate(splashEffectPrefab, splashPosition, Quaternion.identity, null);

            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.waterImpact, splashPosition);
        }
    }
}