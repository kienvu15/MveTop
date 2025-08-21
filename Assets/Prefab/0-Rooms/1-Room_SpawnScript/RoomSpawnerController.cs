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

    [Header("References")]
    [SerializeField] private EnemyDatabase enemyDatabase;
    [SerializeField] private ThemeManager themeManager;

    private List<Transform> activeEnemies = new List<Transform>();
    private int currentWaveIndex = 0;
    private bool roomCleared = false;

    void Start()
    {
        themeManager = FindFirstObjectByType<ThemeManager>();
    }

    // Bắt đầu khi Player bước vào phòng
    public void StartRoom()
    {
        Debug.Log($"[RoomSpawner] Player vào phòng, bắt đầu wave 0.");
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
                SpawnEnemyFromDatabase(enemyInfo.enemyType);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    private void SpawnEnemyFromDatabase(EnemyClass type)
    {
        var enemies = enemyDatabase.GetEnemiesForThemeAndStage(
            themeManager.currentTheme,
            themeManager.stageIndexInTheme
        );

        // Lọc thêm theo loại enemy
        enemies = enemies.FindAll(e => e.enemyType == type);

        if (enemies.Count == 0)
        {
            Debug.LogWarning($"[RoomSpawner] Không tìm thấy enemy loại {type} cho theme {themeManager.currentTheme} stage {themeManager.stageIndexInTheme}.");
            return;
        }

        var chosenEnemy = enemies[Random.Range(0, enemies.Count)];

        Vector3 spawnPos = FindValidSpawnPosition();
        Transform enemy = Instantiate(chosenEnemy.prefab, spawnPos, Quaternion.identity).transform;

        enemy.name = $"{chosenEnemy.enemyName} (Wave {currentWaveIndex})";
        activeEnemies.Add(enemy);

        // 🔹 Gán GridManager cho enemy
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
}
