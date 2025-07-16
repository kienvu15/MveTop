using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Room Settings")]
    public int totalRooms = 10;
    public float roomSpacing = 25f;

    [Header("Prefabs")]
    public List<RoomPrefabData> roomPrefabs;
    public GameObject corridorPrefab;

    private List<Vector2> placedRoomPositions = new List<Vector2>();
    private List<GameObject> spawnedRooms = new List<GameObject>();
    private Dictionary<RoomType, GameObject> prefabDict;

    private void Start()
    {
        BuildPrefabDictionary();
        GenerateDungeon();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RegenerateDungeon();
        }
    }

    public void RegenerateDungeon()
    {
        // Xóa dungeon cũ
        foreach (var room in spawnedRooms)
        {
            Destroy(room);
        }
        spawnedRooms.Clear();
        placedRoomPositions.Clear();

        // Sinh lại dungeon mới
        GenerateDungeon();
    }

    private void BuildPrefabDictionary()
    {
        prefabDict = new Dictionary<RoomType, GameObject>();
        foreach (var room in roomPrefabs)
        {
            prefabDict[room.type] = room.prefab;
        }
    }

    private void GenerateDungeon()
    {
        List<RoomType> roomsToPlace = GenerateRoomList();

        for (int i = 0; i < roomsToPlace.Count; i++)
        {
            Vector2 roomPos = PickRoomPosition();
            GameObject prefab = prefabDict[roomsToPlace[i]];

            GameObject newRoom = Instantiate(prefab, roomPos, Quaternion.identity, transform);
            spawnedRooms.Add(newRoom);

            if (i > 0)
            {
                Vector2 previousRoomPos = placedRoomPositions[placedRoomPositions.Count - 1];
                PlaceCorridor(previousRoomPos, roomPos);
            }

            placedRoomPositions.Add(roomPos);
        }
    }

    private List<RoomType> GenerateRoomList()
    {
        List<RoomType> rooms = new List<RoomType>();

        rooms.Add(RoomType.Shop);
        rooms.Add(RoomType.MiniBoss);
        rooms.Add(RoomType.Special);
        rooms.Add(RoomType.Blessing);

        int normalRoomCount = totalRooms - rooms.Count;
        for (int i = 0; i < normalRoomCount; i++)
        {
            rooms.Add(RoomType.Normal);
        }

        // Shuffle list
        for (int i = 0; i < rooms.Count; i++)
        {
            RoomType temp = rooms[i];
            int randomIndex = Random.Range(i, rooms.Count);
            rooms[i] = rooms[randomIndex];
            rooms[randomIndex] = temp;
        }

        return rooms;
    }

    private Vector2 PickRoomPosition()
    {
        Vector2 pos;
        int attempts = 0;
        do
        {
            float x = Random.Range(-5, 5) * roomSpacing;
            float y = Random.Range(-5, 5) * roomSpacing;
            pos = new Vector2(x, y);

            attempts++;
            if (attempts > 200) break;

        } while (placedRoomPositions.Contains(pos));

        return pos;
    }

    private void PlaceCorridor(Vector2 a, Vector2 b)
    {
        Vector2 corridorPos = (a + b) / 2f;
        Quaternion rotation = (Mathf.Abs(a.x - b.x) > Mathf.Abs(a.y - b.y)) ? Quaternion.identity : Quaternion.Euler(0, 0, 90);

        Instantiate(corridorPrefab, corridorPos, rotation, transform);
    }
}
