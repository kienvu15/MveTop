using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class RoomSpawnerController : MonoBehaviour, IEnemySpawner
{
    [Header("Wave Config")]
    [SerializeField] private RoomWaveConfigSO roomWaveConfig; // 🔹 Asset ScriptableObject
    private List<EnemyWaveConfig> waves;

    [Header("Room Info (hiển thị)")]
    [SerializeField, ReadOnly(true)] private RoomClass chosenRoomClass; // 🔹 hiển thị trên Inspector

    [Header("Spawn Settings")]
    [SerializeField] private Collider2D spawnArea;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float minDistanceFromOtherEnemies = 1.5f;

    [Header("References")]
    [SerializeField] private EnemyDatabase enemyDatabase;
    [SerializeField] private ThemeManager themeManager;

    private List<Transform> activeEnemies = new List<Transform>();
    public int currentWaveIndex = 0;
    public bool roomCleared = false;

    void Start()
    {
        if (themeManager == null)
            themeManager = FindFirstObjectByType<ThemeManager>();
    }

    // Bắt đầu khi Player bước vào phòng
    public void StartRoom()
    {
        if (roomWaveConfig == null || roomWaveConfig.roomConfigs.Count == 0)
        {
            Debug.LogError("[RoomSpawner] Không có RoomWaveConfig!");
            return;
        }

        // 🔹 Random chọn RoomClass
        var config = roomWaveConfig.roomConfigs[Random.Range(0, roomWaveConfig.roomConfigs.Count)];
        chosenRoomClass = config.roomClass;
        waves = config.waves;

        Debug.Log($"[RoomSpawner] Player vào phòng, chọn class {chosenRoomClass}, bắt đầu wave 0.");
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

        Debug.Log($"[BossRoomSpawner] Bắt đầu wave {currentWaveIndex + 1}.");
        StartCoroutine(SpawnWaveCoroutine(waves[currentWaveIndex]));
        currentWaveIndex++;
    }


    private IEnumerator SpawnWaveCoroutine(EnemyWaveConfig wave)
    {
        Debug.Log($"[RoomSpawner] Starting wave {currentWaveIndex + 1} with delay {wave.delayBeforeWave}");
        yield return new WaitForSeconds(wave.delayBeforeWave);
        foreach (var enemyInfo in wave.enemiesToSpawn)
        {
            Debug.Log($"[RoomSpawner] Spawning {enemyInfo.quantity} {enemyInfo.enemyType}");
            for (int i = 0; i < enemyInfo.quantity; i++)
            {
                SpawnEnemy(enemyInfo.enemyType);
                yield return new WaitForSeconds(0.1f);
            }
        }
        Debug.Log($"[RoomSpawner] Finished spawning wave {currentWaveIndex + 1}");
    }

    private void SpawnEnemy(EnemyClass type)
    {
        // 🔹 Lấy enemy hợp theme & stage
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

        // Gán GridManager cho enemy nếu có
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

        var enemyDeath = enemy.GetComponentInChildren<EnemyDeath>();
        if (enemyDeath != null)
        {
            Debug.Log($"[RoomSpawner] SetupSpawner for {enemy.name}");
            enemyDeath.SetupSpawner(this);
        }
        else
        {
            Debug.LogError($"[RoomSpawner] Enemy {enemy.name} missing EnemyDeath component!");
        }


    }

    public void OnEnemyDied(Transform enemy)
    {
        activeEnemies.Remove(enemy);
        Debug.Log($"[RoomSpawner] Removed {enemy.name}. Active enemies left: {activeEnemies.Count}");

        if (activeEnemies.Count <= 0)
        {
            if (currentWaveIndex < waves.Count)
            {
                // còn wave -> spawn tiếp
                StartNextWave();
            }
            else
            {
                // hết wave -> cleared
                Debug.Log("[RoomSpawner] Room Cleared!");
                roomCleared = true;

                var room = GetComponentInParent<RoomController>();
                if (room != null)
                    room.OnRoomCleared();
            }
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
