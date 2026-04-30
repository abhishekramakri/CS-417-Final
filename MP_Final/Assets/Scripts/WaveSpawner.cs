using System.Collections;
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

    [Header("Waves")]
    public WaveData[] waves;
    [Tooltip("Seconds the player gets between wave end and the next wave starting")]
    public float timeBetweenWaves = 15f;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Arrival Animation")]
    public float dropHeight = 3f;
    public float dropDuration = 0.45f;

    int _waveIndex;

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
            yield return SpawnWave(waves[_waveIndex]);
            _waveIndex++;
        }
    }

    IEnumerator SpawnWave(WaveData wave)
    {
        for (int i = 0; i < wave.count; i++)
        {
            Transform spawnPt = spawnPoints[i % spawnPoints.Length];
            Vector3 dropStart = spawnPt.position + Vector3.up * dropHeight;

            GameObject worker = Instantiate(wave.workerPrefab, dropStart, spawnPt.rotation);
            StartCoroutine(ArrivalDrop(worker.transform, spawnPt.position));

            yield return new WaitForSeconds(wave.spawnInterval);
        }
    }

    // Smooth-steps the worker from drop height down to the ground
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
