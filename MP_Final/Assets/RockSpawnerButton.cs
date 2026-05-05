using UnityEngine;

public class RockSpawnerButton : MonoBehaviour
{
    public RockSpawner spawner;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
        {
            spawner.SpawnRock();
        }
    }
}
