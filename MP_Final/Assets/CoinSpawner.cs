using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public GameObject coinPrefab;

    public Vector3 areaCenter;
    public Vector3 areaSize = new Vector3(10f, 0f, 10f);

    public int numberOfCoins = 10;

    void Start()
    {
        SpawnCoins();
    }

    void SpawnCoins()
    {
        for (int i = 0; i < numberOfCoins; i++)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(-areaSize.x / 2, areaSize.x / 2),
                0f,
                Random.Range(-areaSize.z / 2, areaSize.z / 2)
            );

            Vector3 spawnPos = areaCenter + randomPos;

            Instantiate(coinPrefab, spawnPos, Quaternion.identity);
        }
    }
}