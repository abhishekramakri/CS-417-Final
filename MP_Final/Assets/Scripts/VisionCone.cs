using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class VisionCone : MonoBehaviour
{
    [Header("Detection")]
    public float detectionRange = 8f;
    [Tooltip("Full cone angle in degrees (e.g. 60 = 30 each side of forward)")]
    public float detectionAngle = 60f;
    [Tooltip("Must match the Tag on your XR Rig / player root")]
    public string playerTag = "Player";

    [Header("Near-Miss Pulse")]
    public float nearMissRange = 11f;
    public float nearMissAngleMultiplier = 1.4f;
    public Transform coneVisual;
    public float pulseScaleAmount = 0.12f;
    public float pulseSpeed = 4f;

    [Header("Feedback")]
    public AudioClip alertSound;
    public ParticleSystem alertParticles;

    AudioSource _audio;
    Vector3 _coneBaseScale;
    bool _detected;
    Transform _player;

    void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _audio.spatialBlend = 1f;
        _audio.playOnAwake = false;
        if (coneVisual != null)
            _coneBaseScale = coneVisual.localScale;
    }

    void Start()
    {
        // Find the player once by tag so we never need a LayerMask set up
        GameObject playerObj = GameObject.FindWithTag(playerTag);
        if (playerObj != null)
            _player = playerObj.transform;
        else
            Debug.LogWarning($"VisionCone: No GameObject found with tag '{playerTag}'. " +
                             "Set the Tag on your XR Rig to match.");
    }

    void Update()
    {
        if (_detected) return;
        if (_player == null) return;

        float dist  = Vector3.Distance(transform.position, _player.position);
        float angle = Vector3.Angle(transform.forward, _player.position - transform.position);

        bool inDetectionCone = dist <= detectionRange  && angle <= detectionAngle * 0.5f;
        bool inNearMissZone  = dist <= nearMissRange   && angle <= detectionAngle * 0.5f * nearMissAngleMultiplier;

        if (inDetectionCone)
            TriggerDetection();
        else if (inNearMissZone)
            PulseCone();
        else
            SmoothResetCone();
    }

    void TriggerDetection()
    {
        _detected = true;

        if (alertSound != null)
            _audio.PlayOneShot(alertSound);

        if (alertParticles != null)
            alertParticles.Play();

        if (coneVisual != null)
            coneVisual.localScale = _coneBaseScale * (1f + pulseScaleAmount * 2f);

        GetComponent<WorkerEnemy>()?.OnDetected();

        Debug.Log($"VisionCone on {gameObject.name}: PLAYER DETECTED");
    }

    void PulseCone()
    {
        if (coneVisual == null) return;
        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseScaleAmount;
        coneVisual.localScale = _coneBaseScale * pulse;
    }

    void SmoothResetCone()
    {
        if (coneVisual == null) return;
        coneVisual.localScale = Vector3.Lerp(coneVisual.localScale, _coneBaseScale, 10f * Time.deltaTime);
    }

    public bool IsDetected => _detected;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
        DrawConeGizmo(detectionRange, detectionAngle);
        Gizmos.color = new Color(1f, 1f, 0f, 0.12f);
        DrawConeGizmo(nearMissRange, detectionAngle * nearMissAngleMultiplier);
    }

    void DrawConeGizmo(float range, float angle)
    {
        int segments = 20;
        Vector3 prev = transform.position + Quaternion.AngleAxis(-angle * 0.5f, transform.up)
                       * transform.forward * range;
        for (int i = 1; i <= segments; i++)
        {
            float t = (float)i / segments;
            float a = Mathf.Lerp(-angle * 0.5f, angle * 0.5f, t);
            Vector3 next = transform.position + Quaternion.AngleAxis(a, transform.up)
                           * transform.forward * range;
            Gizmos.DrawLine(transform.position, next);
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }
}
