using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "RoomDatabase", menuName = "Game/Room Database")]
public class RoomDatabase : ScriptableObject
{
    [System.Serializable]
    public class RoomSet
    {
        public RoomType type;
        public List<GameObject> prefabs;
    }

    [System.Serializable]
    public class ThemeRoomList
    {
        public ThemeType theme;
        public List<RoomSet> roomsByType;
    }

    public List<ThemeRoomList> allThemes;

    public List<GameObject> GetRooms(ThemeType theme, RoomType type)
    {
        var themeData = allThemes.FirstOrDefault(t => t.theme == theme);
        if (themeData == null) return new List<GameObject>();

        var roomSet = themeData.roomsByType.FirstOrDefault(r => r.type == type);
        if (roomSet == null) return new List<GameObject>();

        return roomSet.prefabs;
    }
}
