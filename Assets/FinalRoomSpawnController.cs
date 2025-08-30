using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class FinalRoomSpawnController : MonoBehaviour, IEnemySpawner
{
    [Header("Core")]
    public bool isFinished = false;

    [Header("Wave Config")]
    [SerializeField] private RoomWaveConfigSO roomWaveConfig; // Asset ScriptableObject chứa các wave config

    [Header("Room Info (hiển thị)")]
    [SerializeField, ReadOnly(true)] private RoomClass chosenRoomClass; // Luôn là Big

    [Header("Spawn Settings")]
    [SerializeField] private Collider2D spawnArea;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float minDistanceFromOtherEnemies = 1.5f;

    [Header("References")]
    [SerializeField] private EnemyDatabase enemyDatabase;
    [SerializeField] private ThemeManager themeManager;
    [SerializeField] private PortalActivatorAfterBoss portalActivator;

    private List<EnemyWaveConfig> waves;
    private List<Transform> activeEnemies = new List<Transform>();
    private int currentWaveIndex = 0;
    private bool roomCleared = false;

    void Start()
    {
        if (themeManager == null)
            themeManager = FindFirstObjectByType<ThemeManager>();
    }

    // Bắt đầu khi Player bước vào phòng Boss
    public void StartRoom()
    {
        if (roomWaveConfig == null || roomWaveConfig.roomConfigs.Count == 0)
        {
            Debug.LogError("[BossRoomSpawner] Không có RoomWaveConfig!");
            return;
        }

        // Ép buộc luôn chọn RoomClass.Big cho phòng Boss
        chosenRoomClass = RoomClass.Big;
        Debug.Log($"[BossRoomSpawner] Player bước vào phòng Boss, ép buộc chọn class {chosenRoomClass}.");

        // Tìm config tương ứng với RoomClass.Big
        var selectedConfig = roomWaveConfig.roomConfigs.Find(config => config.roomClass == RoomClass.Big);
        if (selectedConfig != null)
        {
            waves = selectedConfig.waves;
        }
        else
        {
            Debug.LogWarning("[BossRoomSpawner] Không tìm thấy config cho RoomClass.Big, dùng config đầu tiên.");
            waves = roomWaveConfig.roomConfigs[0].waves; // Fallback
        }

        Debug.Log($"[BossRoomSpawner] Bắt đầu wave 0 với class {chosenRoomClass}.");
        StartNextWave();
    }

    private void StartNextWave()
    {
        if (currentWaveIndex >= waves.Count)
        {
            Debug.Log("[RoomSpawner] Đã hoàn thành tất cả các wave.");
            roomCleared = true;

            var room = GetComponentInParent<RoomFinalController>();
            if (room != null)
                room.OnRoomCleared();

            if (portalActivator != null)
                portalActivator.OnBossDefeated();

            return;
        }

        Debug.Log($"[RoomSpawner] Bắt đầu wave {currentWaveIndex + 1}.");
        StartCoroutine(SpawnWaveCoroutine(waves[currentWaveIndex]));
        currentWaveIndex++;
    }

    private IEnumerator SpawnWaveCoroutine(EnemyWaveConfig wave)
    {
        Debug.Log($"[BossRoomSpawner] Starting wave {currentWaveIndex + 1} with delay {wave.delayBeforeWave}");
        yield return new WaitForSeconds(wave.delayBeforeWave);

        foreach (var enemyInfo in wave.enemiesToSpawn)
        {
            Debug.Log($"[BossRoomSpawner] Spawning {enemyInfo.quantity} {enemyInfo.enemyType}");
            for (int i = 0; i < enemyInfo.quantity; i++)
            {
                SpawnEnemy(enemyInfo.enemyType);
                yield return new WaitForSeconds(0.1f);
            }
        }
        Debug.Log($"[BossRoomSpawner] Finished spawning wave {currentWaveIndex + 1}");
    }

    private void SpawnEnemy(EnemyClass type)
    {
        // Lấy enemy hợp theme & stage từ database
        var enemies = enemyDatabase.GetEnemiesForThemeAndStage(
            themeManager.currentTheme,
            themeManager.stageIndexInTheme
        );

        // Lọc theo loại enemy
        enemies = enemies.FindAll(e => e.enemyType == type);

        if (enemies.Count == 0)
        {
            Debug.LogWarning($"[BossRoomSpawner] Không tìm thấy enemy loại {type} cho theme {themeManager.currentTheme} stage {themeManager.stageIndexInTheme}.");
            return;
        }

        var chosenEnemy = enemies[Random.Range(0, enemies.Count)];

        Vector3 spawnPos = FindValidSpawnPosition();
        Transform enemy = Instantiate(chosenEnemy.prefab, spawnPos, Quaternion.identity).transform;

        enemy.name = $"{chosenEnemy.enemyName} (Boss Wave {currentWaveIndex})";
        activeEnemies.Add(enemy);

        // Gán GridManager của phòng Boss cho enemy
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
            Debug.Log($"[BossRoomSpawner] SetupSpawner for {enemy.name}");
            enemyDeath.SetupSpawner(this);
        }
        else
        {
            Debug.LogError($"[BossRoomSpawner] Enemy {enemy.name} missing EnemyDeath component!");
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

                var room = GetComponentInParent<RoomFinalController>();
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

        Debug.LogWarning("[BossRoomSpawner] Không tìm được vị trí spawn hợp lệ.");
        return transform.position;
    }
}