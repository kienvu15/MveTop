using UnityEngine;

[System.Serializable]
public class DropItem
{
    public GameObject prefab;        // Prefab của vật phẩm
    public float dropWeight = 1f;    // Trọng số (tỉ lệ rớt tương đối)
}

public class EnemyLootDrop : MonoBehaviour
{
    public DropItem[] DropItem;
    public int maxDrop = 3; // Số lượng vật phẩm rớt

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
        for (int i = 0; i < maxDrop; i++)
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
        foreach (var loot in DropItem)
            totalWeight += loot.dropWeight;

        float randomValue = Random.value * totalWeight;
        float cumulative = 0f;

        foreach (var loot in DropItem)
        {
            cumulative += loot.dropWeight;
            if (randomValue <= cumulative)
                return loot.prefab;
        }

        return null; // Trường hợp không có gì
    }
}
