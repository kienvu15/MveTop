using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomWaveConfig", menuName = "Configs/Room Wave Config")]
public class RoomWaveConfigSO : ScriptableObject
{
    [Header("Cấu hình cho từng loại RoomClass")]
    public List<RoomClassConfig> roomConfigs;
}

[System.Serializable]
public class RoomClassConfig
{
    public RoomClass roomClass;

    [Tooltip("Danh sách wave cho RoomClass này")]
    public List<EnemyWaveConfig> waves;
}

[System.Serializable]
public class EnemyWaveConfig
{
    [Tooltip("Tên wave (dùng để debug)")]
    public string waveName;

    [Tooltip("Delay trước khi bắt đầu wave")]
    public float delayBeforeWave = 1f;

    [Tooltip("Danh sách enemy trong wave này")]
    public List<EnemySpawnInfo> enemiesToSpawn;
}

[System.Serializable]
public class EnemySpawnInfo
{
    [Tooltip("Loại enemy muốn spawn (Melee, Ranged, Tank, Boss...)")]
    public EnemyClass enemyType;

    [Tooltip("Số lượng spawn")]
    public int quantity = 1;
}
