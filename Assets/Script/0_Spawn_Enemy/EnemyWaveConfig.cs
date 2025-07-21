using System.Collections.Generic;

[System.Serializable]
public class EnemyWaveConfig
{
    public string waveName;                 // Tên wave (tùy, để debug)
    public float delayBeforeWave = 0.5f;    // Đợi bao lâu trước khi spawn wave
    public List<EnemySpawnInfo> enemiesToSpawn;  // Danh sách loại + số lượng enemy
}
