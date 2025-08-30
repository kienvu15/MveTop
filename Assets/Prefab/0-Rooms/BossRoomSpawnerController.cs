using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomSpawnerController : MonoBehaviour, IEnemySpawner

{
    [Header("Core")]
    public bool isFinished = false;

    [Header("Boss Wave Config")]
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

    [SerializeField] private PortalActivatorAfterBoss portalActivator;

    private List<Transform> activeEnemies = new List<Transform>();
    private int currentWaveIndex = 0;
    private bool roomCleared = false;

    // Bắt đầu khi Player bước vào phòng Boss
    public void StartRoom()
    {
        Debug.Log($"[BossRoomSpawner] Player bước vào phòng Boss, bắt đầu wave 0.");
        StartNextWave();
    }

    private void StartNextWave()
    {
        if (currentWaveIndex >= waves.Count)
        {
            Debug.Log("[BossRoomSpawner] Đã hoàn thành tất cả các wave Boss.");
            roomCleared = true;

            var room = GetComponentInParent<RoomBossController>();
            if (room != null)
                room.OnRoomCleared();

            if (portalActivator != null)
                portalActivator.OnBossDefeated();

            return;
        }

        Debug.Log($"[BossRoomSpawner] Bắt đầu wave {currentWaveIndex + 1}.");
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
            Debug.LogWarning($"[BossRoomSpawner] Prefab cho loại {type} chưa được gán.");
            return;
        }

        Vector3 spawnPos = FindValidSpawnPosition();
        Transform enemy = Instantiate(prefab, spawnPos, Quaternion.identity).transform;

        enemy.name = $"{type} (Boss Wave {currentWaveIndex})";
        activeEnemies.Add(enemy);

        // 🔹 Gán GridManager của phòng Boss này cho enemy
        GridManager roomGrid = GetComponentInParent<GridManager>();
        if (roomGrid != null)
        {
            var avoidPlayer = enemy.GetComponent<AvoidPlayer>();
            if (avoidPlayer != null)
                avoidPlayer.gridManager = roomGrid;

            var enemySteering = enemy.GetComponent<EnemySteering>();
            if (enemySteering != null)
                enemySteering.gridManager = roomGrid;
        }

        //var enemyDeath = enemy.GetComponent<EnemyDeath>();
        //if (enemyDeath != null)
            //enemyDeath.SetupSpawner(this);
    }


    public void OnEnemyDied(Transform enemy)
    {
        activeEnemies.Remove(enemy);

        if (activeEnemies.Count <= 0 && !roomCleared)
        {
            Debug.Log("[BossRoomSpawner] Tất cả enemy đã chết trong Boss Room.");
            roomCleared = true;

            // Kích hoạt portal đến màn Win
            

            StartNextWave(); // hoặc bỏ nếu chỉ có 1 wave
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

        Debug.LogWarning("[BossRoomSpawner] Không tìm được vị trí spawn hợp lệ.");
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
