using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoadPrefabDatabase", menuName = "Database/RoadPrefabDatabase")]
public class RoadPrefabDatabase : ScriptableObject
{
    public List<ThemeRoadData> themeRoads;

    public GameObject GetRoadPrefab(ThemeType theme, RoadDirection dir)
    {
        ThemeRoadData themeData = themeRoads.Find(t => t.theme == theme);
        if (themeData == null)
        {
            Debug.LogWarning($"Không tìm thấy theme: {theme}");
            return null;
        }

        RoadPrefabData roadData = themeData.roadPrefabs.Find(r => r.direction == dir);
        if (roadData == null)
        {
            Debug.LogWarning($"Không tìm thấy road hướng {dir} trong theme {theme}");
            return null;
        }

        return roadData.prefab;
    }
}
