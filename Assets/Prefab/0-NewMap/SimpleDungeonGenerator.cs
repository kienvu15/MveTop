using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDungeonGenerator : MonoBehaviour
{
    [Header("Cài đặt Dungeon")]
    [HideInInspector] public List<RoomPrefabData> starterRooms;
    [HideInInspector] public List<RoomPrefabData> normalRooms;
    [HideInInspector] public List<RoomPrefabData> shopRooms;
    [HideInInspector] public List<RoomPrefabData> blessingRooms;
    [HideInInspector] public List<RoomPrefabData> specialRooms;
    [HideInInspector] public List<RoomPrefabData> miniBossRooms;
    [HideInInspector] public List<RoomPrefabData> bossRooms;

    [SerializeField] private RoomDatabase roomDatabase;
    [SerializeField] private ThemeManager themeManager;

    [SerializeField] private Transform gridTransform;

    public GameObject roadPrefab;
    public GameObject doorBlockerPrefab;

    public int maxRooms = 10;

    private int roomsSpawned = 0;
    private Queue<Transform> doorQueue = new Queue<Transform>();

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private List<Vector3> occupiedPositions = new List<Vector3>();

    private bool shopSpawned = false;
    private bool blessingSpawned = false;
    private bool specialSpawned = false;
    private bool miniBossSpawned = false;
    private bool bossSpawned = false;

    private bool generationComplete = false;
    private List<Transform> blockedDoorPoints = new List<Transform>();
    private HashSet<Vector3> reservedPositions = new HashSet<Vector3>();

    private EndPointValidator endpointValidator;

    // Helper: convert GameObject list -> RoomPrefabData list
    private List<RoomPrefabData> ConvertPrefabsToRoomPrefabData(List<GameObject> prefabs, RoomType type)
    {
        var list = new List<RoomPrefabData>();
        if (prefabs == null) return list;

        foreach (var pf in prefabs)
        {
            if (pf == null) continue;
            RoomPrefabData data = new RoomPrefabData();
            // giả định RoomPrefabData có public fields: prefab (GameObject) và type (RoomType)
            data.prefab = pf;
            data.type = type;
            list.Add(data);
        }
        return list;
    }

    // Cập nhật tất cả các room lists từ RoomDatabase theo theme hiện tại
    private void UpdateRoomListsForCurrentTheme()
    {
        if (roomDatabase == null || themeManager == null)
        {
            // nếu chưa gán DB / ThemeManager thì giữ nguyên các list cũ (fallback)
            Debug.LogWarning("[SimpleDungeonGenerator] RoomDatabase hoặc ThemeManager chưa gán — dùng lists cũ (fallback).");
            return;
        }

        ThemeType theme = themeManager.currentTheme;

        starterRooms = ConvertPrefabsToRoomPrefabData(roomDatabase.GetRooms(theme, RoomType.Starter), RoomType.Starter);
        normalRooms = ConvertPrefabsToRoomPrefabData(roomDatabase.GetRooms(theme, RoomType.Normal), RoomType.Normal);
        shopRooms = ConvertPrefabsToRoomPrefabData(roomDatabase.GetRooms(theme, RoomType.Shop), RoomType.Shop);
        blessingRooms = ConvertPrefabsToRoomPrefabData(roomDatabase.GetRooms(theme, RoomType.Blessing), RoomType.Blessing);
        specialRooms = ConvertPrefabsToRoomPrefabData(roomDatabase.GetRooms(theme, RoomType.Special), RoomType.Special);
        miniBossRooms = ConvertPrefabsToRoomPrefabData(roomDatabase.GetRooms(theme, RoomType.MiniBoss), RoomType.MiniBoss);
        bossRooms = ConvertPrefabsToRoomPrefabData(roomDatabase.GetRooms(theme, RoomType.Boss), RoomType.Boss);
    }

    // Because your RoomType enum spelled "Blessing" not "Blessing"? adjust names exactly
    // (If RoomDatabase.GetRooms returns empty list, Convert returns empty list.)


    void Start()
    {
        endpointValidator = FindFirstObjectByType<EndPointValidator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(GenerateUntilBossRoom());
        }
    }

    IEnumerator GenerateUntilBossRoom()
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

    public void ResetDungeon()
    {
        foreach (var obj in spawnedObjects)
            Destroy(obj);

        spawnedObjects.Clear();
        doorQueue.Clear();
        occupiedPositions.Clear();
        blockedDoorPoints.Clear();

        roomsSpawned = 0;

        shopSpawned = false;
        blessingSpawned = false;
        specialSpawned = false;
        miniBossSpawned = false;
        bossSpawned = false;
        reservedPositions.Clear();
        generationComplete = false;

        

        Debug.Log("<color=yellow>Đã reset dungeon.</color>");
    }

    void GenerateDungeon()
    {
        // cập nhật các list room theo theme hiện tại (quan trọng)
        UpdateRoomListsForCurrentTheme();

        // nếu sau cập nhật starterRooms rỗng -> fallback xử lý
        if (starterRooms == null || starterRooms.Count == 0)
        {
            Debug.LogError("[SimpleDungeonGenerator] Không có starterRooms cho theme hiện tại!");
            return;
        }

        RoomPrefabData startRoomData = starterRooms[Random.Range(0, starterRooms.Count)];
        GameObject startRoom = Instantiate(startRoomData.prefab, Vector3.zero, Quaternion.identity, gridTransform);


        spawnedObjects.Add(startRoom);
        occupiedPositions.Add(Vector3.zero);

        Debug.Log($"<color=green>Spawned Starter Room: {startRoomData.type}</color>");

        foreach (Transform child in startRoom.GetComponentsInChildren<Transform>())
        {
            if (child.CompareTag("DoorPoint"))
                doorQueue.Enqueue(child);
        }

        roomsSpawned++;
        StartCoroutine(TryExpandFromDoorQueue());

        generationComplete = true;
    }

    void HandleBlockedInUseMarkers()
    {
        foreach (var marker in EndPointMarker.AllMarkers)
        {
            if (marker.isBlock && marker.inUse && marker.blockerCollider != null)
            {
                Debug.Log($"🚫 Tắt blocker: {marker.blockerCollider.name}");
                marker.blockerCollider.gameObject.SetActive(false);
            }
        }
    }


    IEnumerator TryExpandFromDoorQueue()
    {
        while (doorQueue.Count > 0 && roomsSpawned < maxRooms)
        {
            Transform doorPoint = doorQueue.Dequeue();

            // 1. Instantiate road
            GameObject road = Instantiate(roadPrefab, doorPoint.position, doorPoint.rotation, gridTransform);
            spawnedObjects.Add(road);

            // 2. Tìm Start & EndPoint
            Transform startPoint = null, endPoint = null;
            foreach (Transform t in road.GetComponentsInChildren<Transform>())
            {
                if (t.CompareTag("StartPoint")) startPoint = t;
                if (t.CompareTag("EndPoint")) endPoint = t;
            }

            // 3. Align road về đúng vị trí (bắt buộc trước validator)
            if (startPoint == null || endPoint == null)
            {
                Debug.LogError("Road Prefab thiếu StartPoint hoặc EndPoint.");
                continue;
            }

            Vector3 offset = startPoint.position - road.transform.position;
            road.transform.position -= offset;

            // ✅ 4. Giờ mới gọi Validator — marker đã ở đúng chỗ!
            if (endpointValidator != null)
                endpointValidator.CheckIntersectionAndReturnInactiveMarkers();

            // 5. Tiếp tục spawn
            TrySpawnRoomAt(endPoint);



            yield return new WaitForSeconds(0.05f);
        }

        if (!bossSpawned)
            ForceSpawnBossRoomFromBlockedDoors();

        foreach (var door in doorQueue)
            BlockUnusedDoor(door);

        doorQueue.Clear();
        StartCoroutine(WaitAndCleanUpAfterGeneration());


    }
    IEnumerator WaitAndCleanUpAfterGeneration()
    {
        // Đợi tất cả các Road và Marker thực sự đã Update ít nhất 1 frame
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame(); // Đảm bảo collider & raycast ổn định

        yield return new WaitForSeconds(0.5f); // Đợi thêm một chút nữa cho chắc chắn

        Debug.Log("<color=orange>🧹 Đang kiểm tra & vô hiệu hóa các Road không dùng...</color>");
        CleanUpUnusedRoads();
        HandleBlockedInUseMarkers();
    }



    void CleanUpUnusedRoads()
    {
        // Sao chép danh sách để tránh lỗi sửa khi lặp
        var objectsCopy = new List<GameObject>(spawnedObjects);

        foreach (GameObject obj in objectsCopy)
        {
            if (obj == null || obj.tag != "Road") continue;

            EndPointMarker marker = obj.GetComponentInChildren<EndPointMarker>(true);
            if (marker != null)
            {
                marker.CheckIfInUse();
                if (!marker.inUse)
                {
                    Debug.Log($"🧱 Vô hiệu hoá Road vì không dùng: {obj.name}");

                    // 👉 Tìm DoorPoint gốc để đặt blocker
                    Transform startPoint = null;
                    foreach (Transform t in obj.GetComponentsInChildren<Transform>())
                    {
                        if (t.CompareTag("StartPoint"))
                        {
                            startPoint = t;
                            break;
                        }
                    }

                    if (startPoint != null)
                    {
                        RaycastHit2D[] hits = Physics2D.RaycastAll(startPoint.position, startPoint.right, 10f);
                        Debug.DrawRay(startPoint.position, startPoint.right * 10f, Color.red, 2f); // Vẽ ray debug

                        bool foundAny = false;
                        foreach (var hit in hits)
                        {
                            if (hit.collider != null && hit.collider.CompareTag("DoorPoint"))
                            {
                                Transform doorPoint = hit.collider.transform;
                                BlockUnusedDoor(doorPoint);
                                Debug.Log($"🚧 Gắn blocker tại DoorPoint gốc: {doorPoint.name}");
                                foundAny = true;
                            }
                        }

                        if (!foundAny)
                        {
                            Debug.LogWarning("⚠️ Không tìm thấy DoorPoint gốc nào cho road: " + obj.name);
                        }
                    }

                    // ❗ Tắt road sau khi dùng xong
                    obj.SetActive(false); // <-- dòng này vẫn an toàn nhờ objectsCopy
                }
            }
        }
    }




    void TrySpawnRoomAt(Transform endPoint)
    {
        if (roomsSpawned >= maxRooms)
        {
            Debug.Log("🚫 Đã đạt maxRooms, huỷ road và thêm blocker tại: " + endPoint.name);

            // Xử lý khi đã spawn road nhưng không được phép spawn room
            if (doorBlockerPrefab != null)
            {
                Instantiate(doorBlockerPrefab, endPoint.position, endPoint.rotation, gridTransform);
                Debug.Log("🚧 Đặt blocker tại: " + endPoint.position);
            }
            else
            {
                Debug.LogWarning("❌ Không có prefab blocker được gán trong Inspector!");
            }


            Transform road = endPoint.parent;
            if (road != null)
            {
                Debug.Log("🧹 Xoá road: " + road.name);
                Destroy(road.gameObject);
            }
            else
            {
                Debug.LogWarning("⚠️ Không tìm thấy parent (road) của endpoint: " + endPoint.name);
            }


            return;
        }


        var marker = endPoint.GetComponent<EndPointMarker>();
        if (marker != null && !marker.isValid)
        {
            Debug.LogWarning($"❌ Bỏ qua EndPoint không hợp lệ: {endPoint.name}");
            return;
        }

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

            if (reservedPositions.Contains(targetPos) || IsPositionOccupied(targetPos))
            {
                Destroy(newRoom);
                continue;
            }

            newRoom.transform.position = targetPos;
            reservedPositions.Add(targetPos);

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
                        doorQueue.Enqueue(child);

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
