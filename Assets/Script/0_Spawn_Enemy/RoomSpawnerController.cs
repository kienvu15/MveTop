using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawnerController : MonoBehaviour, IEnemySpawner

{
    [Header("Wave Config")]
    public List<EnemyWaveConfig> waves;

    [Header("Spawn Settings")]
    [SerializeField] private Collider2D spawnArea;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float minDistanceFromOtherEnemies = 1.5f;

    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject meleePrefab;
    [SerializeField] private GameObject rangedPrefab;
    [SerializeField] private GameObject chargerPrefab;
    [SerializeField] private GameObject miniBossPrefab;
    [SerializeField] private GameObject bossPrefab;

    private List<Transform> activeEnemies = new List<Transform>();
    private int currentWaveIndex = 0;
    private bool roomCleared = false;

    // Bắt đầu khi Player bước vào phòng
    public void StartRoom()
    {
        Debug.Log($"[RoomSpawner] Player bước vào phòng, bắt đầu wave 0.");
        StartNextWave();
    }

    private void StartNextWave()
    {
        if (currentWaveIndex >= waves.Count)
        {
            Debug.Log("[RoomSpawner] Đã hoàn thành tất cả các wave.");
            roomCleared = true;

            var room = GetComponentInParent<RoomController>();
            if (room != null)
                room.OnRoomCleared();

            return;
        }


        Debug.Log($"[RoomSpawner] Bắt đầu wave {currentWaveIndex + 1}.");
        StartCoroutine(SpawnWaveCoroutine(waves[currentWaveIndex]));
        currentWaveIndex++;
    }

    private IEnumerator SpawnWaveCoroutine(EnemyWaveConfig wave)
    {
        yield return new WaitForSeconds(wave.delayBeforeWave);

        foreach (var enemyInfo in wave.enemiesToSpawn)
        {
            for (int i = 0; i < enemyInfo.quantity; i++)
            {
                SpawnEnemy(enemyInfo.enemyType);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    private void SpawnEnemy(EnemyClass type)
    {
        GameObject prefab = GetPrefabByType(type);

        if (prefab == null)
        {
            Debug.LogWarning($"[RoomSpawner] Prefab cho loại {type} chưa được gán.");
            return;
        }

        Vector3 spawnPos = FindValidSpawnPosition();
        Transform enemy = Instantiate(prefab, spawnPos, Quaternion.identity).transform;

        enemy.name = $"{type} (Wave {currentWaveIndex})";
        activeEnemies.Add(enemy);

        var enemyDeath = enemy.GetComponent<EnemyDeath>();
        if (enemyDeath != null)
            enemyDeath.SetupSpawner(this);
    }

    public void OnEnemyDied(Transform enemy)
    {
        activeEnemies.Remove(enemy);

        if (activeEnemies.Count <= 0 && !roomCleared)
        {
            Debug.Log("[RoomSpawner] Tất cả enemy đã chết, chuyển sang wave tiếp theo.");
            StartNextWave();
        }
    }

    private Vector3 FindValidSpawnPosition()
    {
        Bounds bounds = spawnArea.bounds;

        for (int i = 0; i < 30; i++)
        {
            Vector3 candidate = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                0
            );

            bool nearObstacle = Physics2D.OverlapCircle(candidate, 0.4f, obstacleMask);
            if (nearObstacle) continue;

            bool tooCloseToEnemy = false;
            foreach (Transform enemy in activeEnemies)
            {
                if (enemy == null) continue;
                if (Vector3.Distance(candidate, enemy.position) < minDistanceFromOtherEnemies)
                {
                    tooCloseToEnemy = true;
                    break;
                }
            }

            if (!tooCloseToEnemy)
                return candidate;
        }

        Debug.LogWarning("[RoomSpawner] Không tìm được vị trí spawn hợp lệ.");
        return transform.position;
    }

    private GameObject GetPrefabByType(EnemyClass type)
    {
        switch (type)
        {
            case EnemyClass.Melee: return meleePrefab;
            case EnemyClass.Ranged: return rangedPrefab;
            case EnemyClass.Charger: return chargerPrefab;
            case EnemyClass.MiniBoss: return miniBossPrefab;
            case EnemyClass.Boss: return bossPrefab;
            default: return null;
        }
    }
}
