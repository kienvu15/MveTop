using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    private RoomSpawnerController spawner;

    public void SetupSpawner(RoomSpawnerController controller)
    {
        spawner = controller;
    }

    public void Die()
    {
        spawner?.OnEnemyDied(transform);
        Destroy(gameObject);
    }
}
