using UnityEngine;

public class RockSpawner : MonoBehaviour
{
    public GameObject rockPrefab;
    public Transform spawnPoint;

    public void SpawnRock()
    {
        Debug.Log("Rock Spawned");
        Instantiate(rockPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}