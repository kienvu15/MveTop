using System.Collections.Generic;
using UnityEngine;

public class SimpleDungeonGenerator : MonoBehaviour
{
    [Header("Cài đặt Dungeon")]
    public List<RoomPrefabData> roomPrefabs;
    public GameObject roadPrefab;
    public GameObject doorBlockerPrefab;
    public int maxRooms = 7;

    private int roomsSpawned = 0;
    private List<Transform> openDoorPoints = new List<Transform>();
    private List<GameObject> spawnedObjects = new List<GameObject>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ResetDungeon();
            GenerateDungeon();
        }
    }

    void ResetDungeon()
    {
        foreach (var obj in spawnedObjects)
        {
            Destroy(obj);
        }

        spawnedObjects.Clear();
        openDoorPoints.Clear();
        roomsSpawned = 0;

        Debug.Log("<color=yellow>Đã reset dungeon.</color>");
    }

    void GenerateDungeon()
    {
        RoomPrefabData startRoomData = roomPrefabs[Random.Range(0, roomPrefabs.Count)];
        GameObject startRoom = Instantiate(startRoomData.prefab, Vector3.zero, Quaternion.identity);
        spawnedObjects.Add(startRoom);

        Debug.Log($"<color=green>Spawned Start Room: {startRoomData.type}</color>");

        foreach (Transform child in startRoom.GetComponentsInChildren<Transform>())
        {
            if (child.CompareTag("DoorPoint"))
                openDoorPoints.Add(child);
        }

        roomsSpawned++;

        TryExpandFromOpenPoints();
    }

    void TryExpandFromOpenPoints()
    {
        while (openDoorPoints.Count > 0 && roomsSpawned < maxRooms)
        {
            Transform doorPoint = openDoorPoints[0];
            openDoorPoints.RemoveAt(0);

            GameObject road = Instantiate(roadPrefab, doorPoint.position, doorPoint.rotation);
            spawnedObjects.Add(road);

            Transform startPoint = null, endPoint = null;
            foreach (Transform t in road.GetComponentsInChildren<Transform>())
            {
                if (t.CompareTag("StartPoint"))
                    startPoint = t;
                if (t.CompareTag("EndPoint"))
                    endPoint = t;
            }

            if (startPoint == null || endPoint == null)
            {
                Debug.LogError("Road Prefab thiếu StartPoint hoặc EndPoint.");
                continue;
            }

            Vector3 offset = startPoint.position - road.transform.position;
            road.transform.position -= offset;

            SpawnRoomAtEndPoint(endPoint);
        }

        // Sau khi sinh đủ phòng, chặn toàn bộ cửa còn lại
        foreach (var doorPoint in openDoorPoints)
        {
            BlockUnusedDoor(doorPoint);
        }

        openDoorPoints.Clear();
    }

    void SpawnRoomAtEndPoint(Transform endPoint)
    {
        if (roomsSpawned >= maxRooms)
            return;

        List<(RoomPrefabData, Transform)> validRooms = new List<(RoomPrefabData, Transform)>();

        foreach (var roomData in roomPrefabs)
        {
            GameObject preview = Instantiate(roomData.prefab);
            preview.SetActive(false);

            foreach (Transform doorPoint in preview.GetComponentsInChildren<Transform>())
            {
                if (!doorPoint.CompareTag("DoorPoint"))
                    continue;

                Vector3 doorDir = doorPoint.right.normalized;
                Vector3 endDir = endPoint.right.normalized;

                if (Vector3.Dot(doorDir, -endDir) > 0.95f)
                {
                    validRooms.Add((roomData, doorPoint));
                }
            }

            Destroy(preview);
        }

        if (validRooms.Count == 0)
        {
            Debug.LogWarning("❌ Không tìm được phòng phù hợp với EndPoint.");
            return;
        }

        var selected = validRooms[Random.Range(0, validRooms.Count)];
        RoomPrefabData selectedRoomData = selected.Item1;

        GameObject newRoom = Instantiate(selectedRoomData.prefab);
        spawnedObjects.Add(newRoom);

        Transform selectedDoorPoint = null;

        foreach (Transform t in newRoom.GetComponentsInChildren<Transform>())
        {
            if (t.CompareTag("DoorPoint") &&
                Vector3.Dot(t.right.normalized, -endPoint.right.normalized) > 0.95f)
            {
                selectedDoorPoint = t;
                break;
            }
        }

        if (selectedDoorPoint == null)
        {
            Debug.LogError("Không tìm được đúng DoorPoint trong phòng đã spawn.");
            return;
        }

        Vector3 offset = selectedDoorPoint.position - newRoom.transform.position;
        newRoom.transform.position = endPoint.position - offset;

        roomsSpawned++;
        Debug.Log($"<color=cyan>Spawned Room {roomsSpawned}/{maxRooms}: {selectedRoomData.type}</color>");

        foreach (Transform child in newRoom.GetComponentsInChildren<Transform>())
        {
            if (child.CompareTag("DoorPoint") && child != selectedDoorPoint)
            {
                if (roomsSpawned >= maxRooms)
                {
                    BlockUnusedDoor(child);
                }
                else
                {
                    openDoorPoints.Add(child);
                }
            }
        }
    }

    void BlockUnusedDoor(Transform doorPoint)
    {
        GameObject blocker = Instantiate(doorBlockerPrefab, doorPoint.position, doorPoint.rotation);
        blocker.transform.SetParent(doorPoint);
        spawnedObjects.Add(blocker);
    }
}
