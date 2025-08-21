using UnityEngine;

public class EnemyLootDrop : MonoBehaviour
{
    public EnemyLootTable lootTable;   // Gán ScriptableObject LootTable ở Inspector

    private EnemyStats enemyStats;

    private void Awake()
    {
        enemyStats = GetComponent<EnemyStats>();
    }

    private void OnEnable()
    {
        enemyStats.OnDeath += DropLoot;
    }

    private void OnDisable()
    {
        enemyStats.OnDeath -= DropLoot;
    }

    private void DropLoot()
    {
        if (lootTable == null || lootTable.dropItems.Length == 0)
            return;

        for (int i = 0; i < lootTable.maxDrop; i++)
        {
            GameObject lootPrefab = GetRandomLoot();
            if (lootPrefab != null)
            {
                Vector3 spawnPos = transform.position + (Vector3)Random.insideUnitCircle * 0.5f;
                Instantiate(lootPrefab, spawnPos, Quaternion.identity);
            }
        }
    }

    private GameObject GetRandomLoot()
    {
        float totalWeight = 0f;
        foreach (var loot in lootTable.dropItems)
            totalWeight += loot.dropWeight;

        float randomValue = Random.value * totalWeight;
        float cumulative = 0f;

        foreach (var loot in lootTable.dropItems)
        {
            cumulative += loot.dropWeight;
            if (randomValue <= cumulative)
                return loot.prefab;
        }

        return null;
    }
}
