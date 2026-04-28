using UnityEngine;
using UnityEngine.UI;

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

    float _durability;
    int _handsInContact;

    void Start()
    {
        _durability = maxDurability;
        if (progressRingImage != null)
            progressRingImage.fillAmount = 0f;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("PlayerHand")) return;
        _handsInContact++;
        if (gnawParticles != null) gnawParticles.Play();
        if (gnawAudio != null) gnawAudio.Play();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("PlayerHand")) return;
        _handsInContact = Mathf.Max(0, _handsInContact - 1);
        if (_handsInContact == 0)
        {
            if (gnawParticles != null) gnawParticles.Stop();
            if (gnawAudio != null) gnawAudio.Stop();
        }
    }

    void Update()
    {
        if (_handsInContact == 0) return;

        _durability -= Time.deltaTime;

        float fill = 1f - (_durability / maxDurability);
        if (progressRingImage != null)
            progressRingImage.fillAmount = fill;

        if (_durability <= 0f)
            Destroy();
    }

    void Destroy()
    {
        GameObject prefab = materialType == MaterialType.Wood ? woodScrapPrefab : metalScrapPrefab;
        if (prefab != null)
            Instantiate(prefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
