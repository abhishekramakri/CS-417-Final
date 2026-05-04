using UnityEngine;
using TMPro;

public class ForestIntegrityUI : MonoBehaviour
{
    [Header("Reference to Spawner")]
    public PerimeterSpawner spawner;

    [Header("UI Text (TMP)")]
    public TextMeshProUGUI integrityText;

    void Update()
    {
        if (spawner == null || integrityText == null) return;

        integrityText.text = "Forest Integrity: " +
            Mathf.RoundToInt(spawner.forestIntegrity) + "%";
    }

    void UpdateForestIntegrity()
{
    if (!waveActive) return;

    float decay = activeMachines * decayRatePerMachine;
    forestIntegrity -= decay * Time.deltaTime;
    forestIntegrity = Mathf.Clamp(forestIntegrity, 0f, 100f);

    // LOW integrity warning pulse
    if (forestIntegrity < 30f)
    {
        HapticsManager.Instance.Pulse(0.2f, 0.1f);
    }

    // Integrity collapse burst
    if (forestIntegrity <= 0f)
    {
        HapticsManager.Instance.Pulse(1.0f, 0.5f);
    }
}
}