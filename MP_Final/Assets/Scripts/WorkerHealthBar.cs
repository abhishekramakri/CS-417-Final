using UnityEngine;
using UnityEngine.UI;

// Attach to a child Canvas (World Space) on the Worker prefab.
// Call SetFill(0-1) from WorkerEnemy to update the bar.
public class WorkerHealthBar : MonoBehaviour
{
    [Header("References")]
    public Image fillImage;

    [Header("Settings")]
    public float heightAboveWorker = 2.2f;
    public Color fullColor   = Color.green;
    public Color emptyColor  = Color.red;

    Transform _cam;

    void Awake()
    {
        _cam = Camera.main?.transform;
    }

    void LateUpdate()
    {
        // Keep bar above the worker at a fixed world height
        Vector3 worldPos = transform.parent.position + Vector3.up * heightAboveWorker;
        transform.position = worldPos;

        // Billboard: always face the camera
        if (_cam != null)
            transform.rotation = Quaternion.LookRotation(transform.position - _cam.position);
    }

    public void SetFill(float fraction)
    {
        fraction = Mathf.Clamp01(fraction);
        if (fillImage == null) return;
        fillImage.fillAmount = fraction;
        fillImage.color = Color.Lerp(emptyColor, fullColor, fraction);
    }
}
