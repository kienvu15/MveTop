using System.Collections.Generic;

[System.Serializable]
public class ThemeRoadData
{
    public ThemeType theme;                  // Theme (Forest, Dungeon...)
    public List<RoadPrefabData> roadPrefabs; // 4 prefab road theo hướng
}
