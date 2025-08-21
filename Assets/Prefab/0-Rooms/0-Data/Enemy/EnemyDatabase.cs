// EnemyDatabase.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDatabase", menuName = "Game/Enemy Database")]
public class EnemyDatabase : ScriptableObject
{
    public List<EnemyData> allEnemies;

    public List<EnemyData> GetEnemiesForThemeAndStage(ThemeType theme, int stageIndexInTheme)
    {
        List<EnemyData> result = new List<EnemyData>();

        foreach (var enemy in allEnemies)
        {
            if (enemy.theme != theme) continue;
            if (stageIndexInTheme < enemy.minStageInTheme || stageIndexInTheme > enemy.maxStageInTheme) continue;
            result.Add(enemy);
        }

        return result;
    }
}
