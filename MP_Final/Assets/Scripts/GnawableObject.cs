using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable))]
public class GnawableObject : MonoBehaviour
{
    public enum MaterialType { Wood, Metal }

    [Header("Gnaw Settings")]
    public MaterialType materialType = MaterialType.Wood;
    public float maxDurability = 3f;
    public GameObject woodScrapPrefab;
    public GameObject metalScrapPrefab;

    [Header("Feedback")]
    public AudioSource gnawAudio;
    public ParticleSystem gnawParticles;
    public Image progressRingImage;

    [Header("Alert")]
    public float alertRadius = 10f;

    UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable _interactable;
    float _durability;
    bool _isBeingGnawed;
    bool _destroying;

    void Awake()
    {
        _interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        _interactable.selectEntered.AddListener(OnSelectEntered);
        _interactable.selectExited.AddListener(OnSelectExited);
    }

    void Start()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.IsDestroyed(gameObject.name))
        {
            Destroy(gameObject);
            return;
        }

        _durability = maxDurability;
        if (progressRingImage != null)
            progressRingImage.fillAmount = 0f;
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        _isBeingGnawed = true;
        if (gnawParticles != null) gnawParticles.Play();
        if (gnawAudio != null) gnawAudio.Play();
    }

    void OnSelectExited(SelectExitEventArgs args)
    {
        _isBeingGnawed = false;
        if (gnawParticles != null) gnawParticles.Stop();
        if (gnawAudio != null) gnawAudio.Stop();
    }

    void Update()
    {
        if (!_isBeingGnawed || _destroying) return;

        _durability -= Time.deltaTime;

        float fill = 1f - (_durability / maxDurability);
        if (progressRingImage != null)
            progressRingImage.fillAmount = fill;

        if (_durability <= 0f)
        {
            _destroying = true;
            StartCoroutine(SnapAndDestroy());
        }
    }

    System.Collections.IEnumerator SnapAndDestroy()
    {
        if (progressRingImage != null)
        {
            float t = 0f;
            float startFill = progressRingImage.fillAmount;
            while (t < 0.15f)
            {
                t += Time.deltaTime;
                progressRingImage.fillAmount = Mathf.Lerp(startFill, 1f, t / 0.15f);
                yield return null;
            }
            progressRingImage.fillAmount = 1f;
        }

        Vector3 originalScale = transform.localScale;
        float shrinkTime = 0.25f;
        float elapsed = 0f;
        while (elapsed < shrinkTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / shrinkTime;
            float eased = 1f - (t * t);
            transform.localScale = originalScale * eased;
            yield return null;
        }
        transform.localScale = Vector3.zero;

        DestroyFence();
    }

    void DestroyFence()
    {
        SaveManager.Instance?.MarkDestroyed(gameObject.name);
        AlertNearbyWorkers();
        GameObject prefab = materialType == MaterialType.Wood ? woodScrapPrefab : metalScrapPrefab;
        if (prefab != null)
        {
            Renderer r = GetComponentInChildren<Renderer>();
            Vector3 spawnPos = r != null ? r.bounds.center + Vector3.up * 0.3f : transform.position + Vector3.up * 0.3f;
            Instantiate(prefab, spawnPos, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    void AlertNearbyWorkers()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, alertRadius);
        foreach (Collider hit in hits)
            hit.GetComponentInParent<WorkerEnemy>()?.OnDetected();
    }
}
