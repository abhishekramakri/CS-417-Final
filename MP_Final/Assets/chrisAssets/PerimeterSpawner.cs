using System.Collections;
using UnityEngine;

public class PerimeterSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] prefabs;

    [Header("Spawn Settings")]
    public float spawnInterval = 5f;

    [Header("Forest Integrity (0–100)")]
    public float forestIntegrity = 100f;
    public float decayRatePerMachine = 2f;

    [Header("Wave Control")]
    public float waveDuration = 20f;
    public float restDuration = 10f;

    private int activeMachines = 0;
    private bool waveActive = false;

    private float xMin = -30f;
    private float xMax = 30f;
    private float zMin = -40f;
    private float zMax = 20f;

    void Start()
    {
        StartCoroutine(WaveLoop());
    }

    void Update()
    {
        UpdateForestIntegrity();
    }

    // 🌲 Euler integration system
    void UpdateForestIntegrity()
    {
        if (!waveActive) return; // pause between waves

        float decay = activeMachines * decayRatePerMachine;
        forestIntegrity -= decay * Time.deltaTime;

        forestIntegrity = Mathf.Clamp(forestIntegrity, 0f, 100f);

        // Optional debug
        // Debug.Log("Forest Integrity: " + forestIntegrity);
    }

    IEnumerator WaveLoop()
    {
        while (true)
        {
            waveActive = true;

            float timer = 0f;

           
            while (timer < waveDuration)
            {
                SpawnTwoObjects();
                yield return new WaitForSeconds(spawnInterval);
                timer += spawnInterval;
            }

            // ⏸ REST PHASE (no decay)
            waveActive = false;

            yield return new WaitForSeconds(restDuration);
        }
    }

    void SpawnTwoObjects()
    {
        HapticsManager.Instance.Pulse(0.1f, 0.05f);
        if (prefabs == null || prefabs.Length == 0) return;

        for (int i = 0; i < 2; i++)
        {
            GameObject prefab = prefabs[i % prefabs.Length];
            Vector3 pos = GetRandomPerimeterPosition();

            GameObject obj = Instantiate(prefab, pos, Quaternion.identity);

            RegisterMachine(obj);
        }
    }

    void RegisterMachine(GameObject obj)
    {
        activeMachines++;

        Machine m = obj.GetComponent<Machine>();
        if (m != null)
        {
            // Hook into destruction so we track threats properly
            StartCoroutine(TrackMachineLifecycle(obj));
        }
    }

    IEnumerator TrackMachineLifecycle(GameObject obj)
    {
        while (obj != null)
        {
            yield return null;
        }

        activeMachines--;
    }

    Vector3 GetRandomPerimeterPosition()
    {
        int edge = Random.Range(0, 4);

        return edge switch
        {
            0 => new Vector3(Random.Range(xMin, xMax), 2f, zMax),
            1 => new Vector3(Random.Range(xMin, xMax), 2f, zMin),
            2 => new Vector3(xMin, 2f, Random.Range(zMin, zMax)),
            _ => new Vector3(xMax, 2f, Random.Range(zMin, zMax)),
        };
    }
}

// using System.Collections;
// using UnityEngine;

// public class PerimeterSpawner : MonoBehaviour
// {
//     public GameObject[] prefabs;
//     public float spawnInterval = 5f;

//     private float xMin = -30f;
//     private float xMax = 30f;
//     private float zMin = -40f;
//     private float zMax = 20f;

//     void Start()
//     {
//         StartCoroutine(SpawnLoop());
//     }

//     IEnumerator SpawnLoop()
//     {
//         while (true)
//         {
//             SpawnTwo();
//             yield return new WaitForSeconds(spawnInterval);
//         }
//     }

//     void SpawnTwo()
//     {
//         if (prefabs == null || prefabs.Length == 0) return;

//         for (int i = 0; i < 2; i++)
//         {
//             GameObject prefab = prefabs[i % prefabs.Length];

//             Vector3 pos = GetRandomEdgePosition();

//             GameObject obj = Instantiate(prefab, pos, Quaternion.identity);

//             obj.transform.position = new Vector3(
//                 obj.transform.position.x,
//                 2f,
//                 obj.transform.position.z
//             );
//         }
//     }

//     Vector3 GetRandomEdgePosition()
//     {
//         int edge = Random.Range(0, 4);

//         return edge switch
//         {
//             0 => new Vector3(Random.Range(xMin, xMax), 2f, zMax),
//             1 => new Vector3(Random.Range(xMin, xMax), 2f, zMin),
//             2 => new Vector3(xMin, 2f, Random.Range(zMin, zMax)),
//             _ => new Vector3(xMax, 2f, Random.Range(zMin, zMax)),
//         };
//     }
// }

// // using System.Collections;
// // using UnityEngine;

// // public class PerimeterSpawner : MonoBehaviour
// // {
// //     [Header("Prefabs to Spawn (assign 2 in Inspector)")]
// //     public GameObject[] prefabs;

// //     [Header("Spawn Settings")]
// //     public float spawnInterval = 5f;

// //     // Rectangle bounds (X-Z plane)
// //     private float xMin = -30f;
// //     private float xMax = 30f;
// //     private float zMin = -40f;
// //     private float zMax = 20f;

// //     void Start()
// //     {
// //         StartCoroutine(SpawnLoop());
// //     }

// //     IEnumerator SpawnLoop()
// //     {
// //         while (true)
// //         {
// //             SpawnTwoObjects();
// //             yield return new WaitForSeconds(spawnInterval);
// //         }
// //     }

// //     void SpawnTwoObjects()
// //     {
// //         if (prefabs == null || prefabs.Length == 0) return;

// //         for (int i = 0; i < 2; i++)
// //         {
// //             GameObject prefab = prefabs[Mathf.Min(i, prefabs.Length - 1)];

// //             Vector3 spawnPos = GetRandomPerimeterPosition();

// //             GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity);

// //             // 🔒 Force Y = 0 on the actual spawned object (extra safety)
// //             Vector3 p = obj.transform.position;
// //             p.y = 0f;
// //             obj.transform.position = p;
// //         }
// //     }

// //     Vector3 GetRandomPerimeterPosition()
// //     {
// //         int edge = Random.Range(0, 4);

// //         switch (edge)
// //         {
// //             case 0: // top
// //                 return new Vector3(Random.Range(xMin, xMax), 0f, zMax);

// //             case 1: // bottom
// //                 return new Vector3(Random.Range(xMin, xMax), 0f, zMin);

// //             case 2: // left
// //                 return new Vector3(xMin, 0f, Random.Range(zMin, zMax));

// //             case 3: // right
// //                 return new Vector3(xMax, 0f, Random.Range(zMin, zMax));
// //         }

// //         return Vector3.zero;
// //     }
// // }