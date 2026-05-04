using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages wave-based spawning of workers at the forest edge.
// Workers drop in from above (dramatic arrival animation) at each spawn point.
// Attach to an empty GameObject in the scene and wire up prefabs and spawn points.
public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct WaveData
    {
        public GameObject workerPrefab;
        public int count;
        [Tooltip("Seconds between individual spawns within this wave")]
        public float spawnInterval;
    }

    // One WaypointRoute per patrol path in the scene.
    // Assign scene waypoint Transforms here instead of inside the prefab.
    [System.Serializable]
    public class WaypointRoute
    {
        public Transform[] waypoints;
    }

    [Header("Waves")]
    public WaveData[] waves;
    [Tooltip("Seconds the player gets between wave end and the next wave starting")]
    public float timeBetweenWaves = 15f;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Patrol Routes")]
    [Tooltip("One route per spawn point. Workers assigned to that spawn point will use that route.")]
    public WaypointRoute[] waypointRoutes;

    [Header("Dam")]
    [Tooltip("Drag the Dam GameObject here so spawned workers know where to attack.")]
    public Transform damTarget;

    [Header("Arrival Animation")]
    public float dropHeight = 3f;
    public float dropDuration = 0.45f;

    int _waveIndex;
    readonly List<GameObject> _activeWorkers = new List<GameObject>();

    void Start()
    {
        if (waves.Length > 0)
            StartCoroutine(RunWaves());
    }

    IEnumerator RunWaves()
    {
        while (_waveIndex < waves.Length)
        {
            yield return new WaitForSeconds(timeBetweenWaves);
            ClearLeftoverWorkers();
            yield return SpawnWave(waves[_waveIndex]);
            _waveIndex++;
        }
    }

    IEnumerator SpawnWave(WaveData wave)
    {
        for (int i = 0; i < wave.count; i++)
        {
            int spawnIndex = i % spawnPoints.Length;
            Transform spawnPt = spawnPoints[spawnIndex];
            Vector3 dropStart = spawnPt.position + Vector3.up * dropHeight;

            GameObject worker = Instantiate(wave.workerPrefab, dropStart, spawnPt.rotation);
            _activeWorkers.Add(worker);

            // Assign patrol waypoints from the scene (avoids the prefab mismatched-type error)
            if (waypointRoutes != null && waypointRoutes.Length > 0)
            {
                WaypointRoute route = waypointRoutes[spawnIndex % waypointRoutes.Length];
                worker.GetComponent<WorkerPatrol>()?.SetWaypoints(route.waypoints);
            }

            // Wire the dam target so workers know where to attack on detection
            var enemy = worker.GetComponent<WorkerEnemy>();
            if (enemy != null && damTarget != null)
                enemy.damTarget = damTarget;

            StartCoroutine(ArrivalDrop(worker.transform, spawnPt.position));

            yield return new WaitForSeconds(wave.spawnInterval);
        }
    }

    void ClearLeftoverWorkers()
    {
        foreach (var w in _activeWorkers)
        {
            if (w != null) Destroy(w);
        }
        _activeWorkers.Clear();
    }

    IEnumerator ArrivalDrop(Transform worker, Vector3 landPos)
    {
        Vector3 startPos = worker.position;
        float elapsed = 0f;
        while (elapsed < dropDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / dropDuration);
            if (worker != null)
                worker.position = Vector3.Lerp(startPos, landPos, t);
            yield return null;
        }
        if (worker != null)
            worker.position = landPos;
    }

    public int CurrentWaveIndex => _waveIndex;
    public bool AllWavesComplete => _waveIndex >= waves.Length;
}
