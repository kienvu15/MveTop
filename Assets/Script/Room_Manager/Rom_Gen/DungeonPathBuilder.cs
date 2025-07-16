//using System.Collections.Generic;
//using UnityEngine;

//public class DungeonPathBuilder : MonoBehaviour
//{
//    [Header("Prefabs")]
//    public List<RoomPrefabData> roomPrefabs;

//    private HashSet<Vector2Int> occupiedPositions = new HashSet<Vector2Int>();
//    private List<GameObject> spawnedRooms = new List<GameObject>();
//    private Dictionary<RoomType, GameObject> prefabDict;

//    private Vector2Int currentPos;
//    private Vector2Int lastDirection;
//    private List<RoomType> roomsToPlace;

//    [Header("Room Count")]
//    public int minRooms = 8;
//    public int maxRooms = 15;

//    private void Start()
//    {
//        BuildPrefabDictionary();
//        GenerateDungeon();
//    }

//    private void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.R))
//        {
//            RegenerateDungeon();
//        }
//    }

//    private void BuildPrefabDictionary()
//    {
//        prefabDict = new Dictionary<RoomType, GameObject>();
//        foreach (var data in roomPrefabs)
//        {
//            prefabDict[data.type] = data.prefab;
//        }
//    }

//    public void GenerateDungeon()
//    {
//        Transform player = GameObject.FindWithTag("Player").transform;

//        currentPos = Vector2Int.RoundToInt(player.position);
//        lastDirection = Vector2Int.zero;

//        int totalRooms = Random.Range(minRooms, maxRooms + 1);
//        roomsToPlace = GenerateRoomSequence(totalRooms);

//        SpawnRoom(currentPos, roomsToPlace[0]);  // Spawn phòng đầu tiên

//        for (int i = 1; i < roomsToPlace.Count; i++)
//        {
//            Vector2Int dir = Vector2Int.zero;
//            Vector2Int corridorPos = Vector2Int.zero;
//            Vector2Int nextRoomPos = Vector2Int.zero;

//            bool foundDirection = false;

//            for (int attempt = 0; attempt < 10; attempt++)
//            {
//                dir = PickNextDirection();
//                corridorPos = currentPos + dir;
//                nextRoomPos = corridorPos + dir;

//                if (!occupiedPositions.Contains(corridorPos) && !occupiedPositions.Contains(nextRoomPos))
//                {
//                    foundDirection = true;
//                    break;
//                }
//            }

//            if (!foundDirection)
//            {
//                Debug.LogWarning($"Room {i}: Không tìm được hướng hợp lệ, dừng lại.");
//                break;
//            }

//            // Sử dụng đúng hướng đã tìm được
//            SpawnRoom(corridorPos, RoomType.Corridor);
//            SpawnRoom(nextRoomPos, roomsToPlace[i]);

//            currentPos = nextRoomPos;
//            lastDirection = dir;  // Cập nhật hướng cuối cùng
//        }
//    }

//    private List<RoomType> GenerateRoomSequence(int totalRooms)
//    {
//        List<RoomType> list = new List<RoomType>
//        {
//            RoomType.Shop,
//            RoomType.MiniBoss,
//            RoomType.Special,
//            RoomType.Blessing
//        };

//        int normalRooms = totalRooms - list.Count;
//        for (int i = 0; i < normalRooms; i++)
//        {
//            list.Add(RoomType.Normal);
//        }

//        for (int i = 0; i < list.Count; i++)
//        {
//            RoomType temp = list[i];
//            int rand = Random.Range(i, list.Count);
//            list[i] = list[rand];
//            list[rand] = temp;
//        }

//        return list;
//    }

//    private Vector2Int PickNextDirection()
//    {
//        List<Vector2Int> directions = new List<Vector2Int>
//        {
//            Vector2Int.left,
//            Vector2Int.right,
//            Vector2Int.up,
//            Vector2Int.down
//        };

//        directions.Remove(-lastDirection);

//        return directions[Random.Range(0, directions.Count)];
//    }

//    private void SpawnRoom(Vector2Int pos, RoomType type)
//    {
//        if (occupiedPositions.Contains(pos)) return;

//        GameObject prefab = prefabDict[type];
//        GameObject room = Instantiate(prefab, (Vector2)pos, Quaternion.identity, transform);

//        spawnedRooms.Add(room);
//        occupiedPositions.Add(pos);
//    }

//    public void RegenerateDungeon()
//    {
//        foreach (var obj in spawnedRooms)
//        {
//            Destroy(obj);
//        }

//        spawnedRooms.Clear();
//        occupiedPositions.Clear();

//        GenerateDungeon();
//    }
//}
