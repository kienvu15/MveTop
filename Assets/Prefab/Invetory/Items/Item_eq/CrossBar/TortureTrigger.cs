using System.Collections.Generic;
using UnityEngine;

public class TortureTrigger : MonoBehaviour
{
    [Header("Prefab spawn khi enemy chết")]
    public GameObject spawnPrefab;

    private List<EnemyStats> enemies = new List<EnemyStats>();

    private void OnEnable()
    {
        enemies.Clear();
        enemies.AddRange(
            FindObjectsByType<EnemyStats>(FindObjectsSortMode.None)
        );

        foreach (var enemy in enemies)
        {
            enemy.OnDeath += () => OnEnemyDeath(enemy);
        }
    }

    private void OnDisable()
    {
        foreach (var enemy in enemies)
        {
            if (enemy != null)
                enemy.OnDeath -= () => OnEnemyDeath(enemy);
        }
        enemies.Clear();
    }

    private void OnEnemyDeath(EnemyStats enemy)
    {
        if (spawnPrefab != null)
            Instantiate(spawnPrefab, enemy.transform.position, Quaternion.identity);

        enemies.Remove(enemy); // dọn khỏi list
    }
}
