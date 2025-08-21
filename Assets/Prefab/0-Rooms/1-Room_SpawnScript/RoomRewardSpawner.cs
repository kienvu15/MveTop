using UnityEngine;

public class RoomRewardSpawner : MonoBehaviour
{
    public GameObject rewardChestPrefab;
    public Transform rewardSpawnPoint;

    public void SpawnReward()
    {
        if (rewardChestPrefab != null && rewardSpawnPoint != null)
        {
            Instantiate(rewardChestPrefab, rewardSpawnPoint.position, Quaternion.identity);
        }
    }
}
