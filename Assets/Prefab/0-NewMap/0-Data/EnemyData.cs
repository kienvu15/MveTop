// EnemyData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public ThemeType theme;
    public EnemyClass enemyType;
    public GameObject prefab;

    [Header("Stage Range in Theme (1-3)")]
    [Range(1, 3)] public int minStageInTheme = 1;
    [Range(1, 3)] public int maxStageInTheme = 3;
}
