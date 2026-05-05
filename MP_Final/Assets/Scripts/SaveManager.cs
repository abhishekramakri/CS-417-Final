using UnityEngine;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [Header("Scrap Prefabs (assign in Inspector)")]
    public GameObject woodScrapPrefab;
    public GameObject metalScrapPrefab;

    const string DestroyedKey = "DestroyedObjects";
    const string PlayerPosKey = "PlayerPosition";
    const string ScrapsKey    = "Scraps";

    HashSet<string> _destroyedIds = new HashSet<string>();

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        Load();
    }

    void Start()
    {
        // Restore player position
        Vector3? saved = GetSavedPlayerPosition();
        if (saved.HasValue)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
                player.transform.position = saved.Value;
        }

        // Re-spawn any scraps that were on the ground when the game was saved
        SpawnSavedScraps();
    }

    // ── Destroyed objects ──────────────────────────────────────────────────

    void Load()
    {
        _destroyedIds.Clear();
        string data = PlayerPrefs.GetString(DestroyedKey, "");
        if (string.IsNullOrEmpty(data)) return;
        foreach (string id in data.Split(','))
            if (!string.IsNullOrEmpty(id))
                _destroyedIds.Add(id);
    }

    public bool IsDestroyed(string id) => _destroyedIds.Contains(id);

    public void MarkDestroyed(string id) => _destroyedIds.Add(id);

    // ── Scrap persistence ──────────────────────────────────────────────────

    // Called on load — spawns scraps that were lying on the ground at save time
    void SpawnSavedScraps()
    {
        string data = PlayerPrefs.GetString(ScrapsKey, "");
        if (string.IsNullOrEmpty(data)) return;

        foreach (string entry in data.Split('|'))
        {
            if (string.IsNullOrEmpty(entry)) continue;

            // Format: "Wood:x,y,z"  or  "Metal:x,y,z"
            string[] halves = entry.Split(':');
            if (halves.Length != 2) continue;

            string typeName = halves[0];
            string[] coords = halves[1].Split(',');
            if (coords.Length != 3) continue;

            if (!float.TryParse(coords[0], out float x) ||
                !float.TryParse(coords[1], out float y) ||
                !float.TryParse(coords[2], out float z)) continue;

            GameObject prefab = typeName == "Metal" ? metalScrapPrefab : woodScrapPrefab;
            if (prefab != null)
                Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity);
        }
    }

    // ── Save ───────────────────────────────────────────────────────────────

    public void Save(Transform playerTransform)
    {
        // Destroyed objects
        PlayerPrefs.SetString(DestroyedKey, string.Join(",", _destroyedIds));

        // Player position
        if (playerTransform != null)
        {
            Vector3 p = playerTransform.position;
            PlayerPrefs.SetString(PlayerPosKey, $"{p.x},{p.y},{p.z}");
        }

        // Scrap positions — scan every Scrap in the scene right now
        var scraps = FindObjectsByType<Scrap>(FindObjectsSortMode.None);
        var entries = new List<string>(scraps.Length);
        foreach (Scrap s in scraps)
        {
            Vector3 p = s.transform.position;
            entries.Add($"{s.scrapType}:{p.x},{p.y},{p.z}");
        }
        PlayerPrefs.SetString(ScrapsKey, string.Join("|", entries));

        PlayerPrefs.Save();
    }

    void OnApplicationQuit()
    {
        GameObject player = GameObject.FindWithTag("Player");
        Save(player != null ? player.transform : null);
    }

    // ── Clear ──────────────────────────────────────────────────────────────

    public void ClearSave()
    {
        PlayerPrefs.DeleteKey(DestroyedKey);
        PlayerPrefs.DeleteKey(PlayerPosKey);
        PlayerPrefs.DeleteKey(ScrapsKey);
        PlayerPrefs.Save();
        _destroyedIds.Clear();
    }

    // ── Helpers ────────────────────────────────────────────────────────────

    Vector3? GetSavedPlayerPosition()
    {
        string data = PlayerPrefs.GetString(PlayerPosKey, "");
        if (string.IsNullOrEmpty(data)) return null;
        string[] parts = data.Split(',');
        if (parts.Length != 3) return null;
        if (float.TryParse(parts[0], out float x) &&
            float.TryParse(parts[1], out float y) &&
            float.TryParse(parts[2], out float z))
            return new Vector3(x, y, z);
        return null;
    }
}
