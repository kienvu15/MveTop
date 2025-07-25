using System.Collections.Generic;
using UnityEngine;

public class SimpleDungeonGenerator : MonoBehaviour
{
    [Header("Cài đặt Dungeon")]
    public List<RoomPrefabData> starterRooms;
    public List<RoomPrefabData> normalRooms;
    public List<RoomPrefabData> shopRooms;
    public List<RoomPrefabData> blessingRooms;
    public List<RoomPrefabData> specialRooms;
    public List<RoomPrefabData> miniBossRooms;
    public List<RoomPrefabData> bossRooms;

    [SerializeField] private Transform gridTransform;

    public GameObject roadPrefab;
    public GameObject doorBlockerPrefab;

    public int maxRooms = 10;

    private int roomsSpawned = 0;
    private List<Transform> openDoorPoints = new List<Transform>();
    private List<GameObject> spawnedObjects = new List<GameObject>();
    private List<Vector3> occupiedPositions = new List<Vector3>();

    private bool shopSpawned = false;
    private bool blessingSpawned = false;
    private bool specialSpawned = false;
    private bool miniBossSpawned = false;
    private bool bossSpawned = false;

    private bool generationComplete = false;
    private List<Transform> blockedDoorPoints = new List<Transform>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(GenerateUntilBossRoom());
        }
    }

    System.Collections.IEnumerator GenerateUntilBossRoom()
    {
        while (true)
        {
            ResetDungeon();
            GenerateDungeon();

            yield return null;

            if (bossSpawned)
            {
                Debug.Log("<color=green>✔️ Đã có BossRoom, kết thúc gen.</color>");
                break;
            }
            else
            {
                Debug.LogWarning("❌ Không có BossRoom. Reset toàn bộ và gen lại...");
            }
        }
    }

    void ResetDungeon()
    {
        foreach (var obj in spawnedObjects)
            Destroy(obj);

        spawnedObjects.Clear();
        openDoorPoints.Clear();
        occupiedPositions.Clear();
        blockedDoorPoints.Clear();

        roomsSpawned = 0;

        shopSpawned = false;
        blessingSpawned = false;
        specialSpawned = false;
        miniBossSpawned = false;
        bossSpawned = false;

        generationComplete = false;

        Debug.Log("<color=yellow>Đã reset dungeon.</color>");
    }

    void GenerateDungeon()
    {
        RoomPrefabData startRoomData = starterRooms[Random.Range(0, starterRooms.Count)];
        GameObject startRoom = Instantiate(startRoomData.prefab, Vector3.zero, Quaternion.identity, gridTransform);

        spawnedObjects.Add(startRoom);
        occupiedPositions.Add(Vector3.zero);

        Debug.Log($"<color=green>Spawned Starter Room: {startRoomData.type}</color>");

        foreach (Transform child in startRoom.GetComponentsInChildren<Transform>())
        {
            if (child.CompareTag("DoorPoint"))
                openDoorPoints.Add(child);
        }

        roomsSpawned++;
        TryExpandFromOpenPoints();
        generationComplete = true;
    }

    void TryExpandFromOpenPoints()
    {
        while (openDoorPoints.Count > 0 && roomsSpawned < maxRooms)
        {
            Transform doorPoint = openDoorPoints[0];
            openDoorPoints.RemoveAt(0);

            // ✅ SỬA: set gridTransform làm cha của road
            GameObject road = Instantiate(roadPrefab, doorPoint.position, doorPoint.rotation, gridTransform);
            spawnedObjects.Add(road);

            Transform startPoint = null, endPoint = null;
            foreach (Transform t in road.GetComponentsInChildren<Transform>())
            {
                if (t.CompareTag("StartPoint")) startPoint = t;
                if (t.CompareTag("EndPoint")) endPoint = t;
            }

            if (startPoint == null || endPoint == null)
            {
                Debug.LogError("Road Prefab thiếu StartPoint hoặc EndPoint.");
                continue;
            }

            Vector3 offset = startPoint.position - road.transform.position;
            road.transform.position -= offset;

            TrySpawnRoomAt(endPoint);
        }

        if (!bossSpawned)
            ForceSpawnBossRoomFromBlockedDoors();

        foreach (var doorPoint in openDoorPoints)
            BlockUnusedDoor(doorPoint);

        openDoorPoints.Clear();
    }

    void TrySpawnRoomAt(Transform endPoint)
    {
        if (roomsSpawned >= maxRooms)
            return;

        List<RoomPrefabData> roomList = ChooseRoomType();
        if (roomList == null || roomList.Count == 0)
            return;

        TrySpawnFromRoomList(roomList, endPoint);
    }

    bool TrySpawnFromRoomList(List<RoomPrefabData> roomList, Transform endPoint)
    {
        List<RoomPrefabData> shuffledList = new List<RoomPrefabData>(roomList);
        ShuffleList(shuffledList);

        foreach (var roomData in shuffledList)
        {
            GameObject preview = Instantiate(roomData.prefab);
            preview.SetActive(false);

            Transform matchedDoor = null;
            foreach (Transform doorPoint in preview.GetComponentsInChildren<Transform>())
            {
                if (doorPoint.CompareTag("DoorPoint") &&
                    Vector3.Dot(doorPoint.right.normalized, -endPoint.right.normalized) > 0.95f)
                {
                    matchedDoor = doorPoint;
                    break;
                }
            }
            Destroy(preview);

            if (matchedDoor == null)
                continue;

            GameObject newRoom = Instantiate(roomData.prefab, Vector3.zero, Quaternion.identity, gridTransform);

            Transform actualDoor = null;
            foreach (Transform t in newRoom.GetComponentsInChildren<Transform>())
            {
                if (t.CompareTag("DoorPoint") &&
                    Vector3.Dot(t.right.normalized, -endPoint.right.normalized) > 0.95f)
                {
                    actualDoor = t;
                    break;
                }
            }

            if (actualDoor == null)
            {
                Destroy(newRoom);
                continue;
            }

            Vector3 offset = actualDoor.position - newRoom.transform.position;
            Vector3 targetPos = endPoint.position - offset;

            if (IsPositionOccupied(targetPos))
            {
                Destroy(newRoom);
                continue;
            }

            newRoom.transform.position = targetPos;

            spawnedObjects.Add(newRoom);
            occupiedPositions.Add(newRoom.transform.position);

            roomsSpawned++;

            Debug.Log($"<color=cyan>Spawned Room {roomsSpawned}/{maxRooms}: {newRoom.name}</color>");

            if (roomList == bossRooms)
            {
                bossSpawned = true;
                Debug.Log("<color=red>⚔️ Đã spawn BossRoom!</color>");
            }

            foreach (Transform child in newRoom.GetComponentsInChildren<Transform>())
            {
                if (child.CompareTag("DoorPoint") && child != actualDoor)
                {
                    if (roomsSpawned >= maxRooms)
                        BlockUnusedDoor(child);
                    else
                        openDoorPoints.Add(child);
                }
            }

            return true;
        }

        return false;
    }

    void ForceSpawnBossRoomFromBlockedDoors()
    {
        if (bossSpawned || blockedDoorPoints.Count == 0)
            return;

        Debug.LogWarning("⚠️ Đang cố gắng ép spawn BossRoom từ các cửa bị chặn.");

        foreach (Transform blockedDoor in blockedDoorPoints.ToArray())
        {
            foreach (Transform child in blockedDoor)
                Destroy(child.gameObject);

            blockedDoorPoints.Remove(blockedDoor);

            // ✅ SỬA: set gridTransform làm cha của road
            GameObject road = Instantiate(roadPrefab, blockedDoor.position, blockedDoor.rotation, gridTransform);
            spawnedObjects.Add(road);

            Transform startPoint = null, endPoint = null;
            foreach (Transform t in road.GetComponentsInChildren<Transform>())
            {
                if (t.CompareTag("StartPoint")) startPoint = t;
                if (t.CompareTag("EndPoint")) endPoint = t;
            }

            if (startPoint != null && endPoint != null)
            {
                Vector3 offset = startPoint.position - road.transform.position;
                road.transform.position -= offset;

                if (TrySpawnFromRoomList(bossRooms, endPoint))
                {
                    bossSpawned = true;
                    return;
                }
            }
        }

        Debug.LogError("❌ Không thể ép spawn BossRoom từ bất kỳ cửa bị chặn nào!");
    }

    List<RoomPrefabData> ChooseRoomType()
    {
        float bossRoomThreshold = 0.6f;
        int minRoomsBeforeBoss = Mathf.FloorToInt(maxRooms * bossRoomThreshold);

        if (!bossSpawned && roomsSpawned >= minRoomsBeforeBoss)
            return bossRooms;

        if (!shopSpawned && Random.value < 0.1f)
        {
            shopSpawned = true;
            return shopRooms;
        }

        if (!blessingSpawned && Random.value < 0.1f)
        {
            blessingSpawned = true;
            return blessingRooms;
        }

        if (!specialSpawned && Random.value < 0.05f)
        {
            specialSpawned = true;
            return specialRooms;
        }

        if (!miniBossSpawned && Random.value < 0.05f)
        {
            miniBossSpawned = true;
            return miniBossRooms;
        }

        return normalRooms;
    }

    void BlockUnusedDoor(Transform doorPoint)
    {
        GameObject blocker = Instantiate(doorBlockerPrefab, doorPoint.position, doorPoint.rotation);
        blocker.transform.SetParent(doorPoint);
        spawnedObjects.Add(blocker);

        blockedDoorPoints.Add(doorPoint);
    }

    bool IsPositionOccupied(Vector3 pos)
    {
        foreach (var p in occupiedPositions)
        {
            if (Vector3.Distance(p, pos) < 0.5f)
                return true;
        }
        return false;
    }

    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
