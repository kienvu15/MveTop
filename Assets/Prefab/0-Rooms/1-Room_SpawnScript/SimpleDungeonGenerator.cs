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
    [HideInInspector] public List<RoomPrefabData> finalRooms; // Thêm finalRooms
    [HideInInspector] public List<RoomPrefabData> bossRooms;

    [SerializeField] public bool isGeneratingDone = false;

    [SerializeField] private RoomDatabase roomDatabase;
    [SerializeField] private RoadPrefabDatabase roadDatabase;
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

    // FIX: dùng List + so sánh gần đúng để tránh sai số float khi check vị trí đã reserve
    private List<Vector3> reservedPositions = new List<Vector3>();
    private const float RESERVED_TOLERANCE = 0.05f;

    private EndPointValidator endpointValidator;

    // ===================== Helpers (FIXED / NEW) =====================
    private bool ContainsReserved(Vector3 pos)
    {
        for (int i = 0; i < reservedPositions.Count; i++)
        {
            if ((reservedPositions[i] - pos).sqrMagnitude <= RESERVED_TOLERANCE * RESERVED_TOLERANCE)
                return true;
        }
        return false;
    }
    private void AddReserved(Vector3 pos)
    {
        if (!ContainsReserved(pos)) reservedPositions.Add(pos);
    }

    private int SnapAngleToRightAngle(float z)
    {
        float angle = z % 360f; if (angle < 0f) angle += 360f;
        int snapped = Mathf.RoundToInt(angle / 90f) * 90;
        snapped %= 360; if (snapped < 0) snapped += 360;
        return snapped; // 0, 90, 180, 270
    }

    private bool HasChildWithTag(Transform parent, string tag)
    {
        foreach (Transform t in parent)
        {
            if (t.CompareTag(tag)) return true;
        }
        return false;
    }

    // Helper: convert GameObject list -> RoomPrefabData list
    private List<RoomPrefabData> ConvertPrefabsToRoomPrefabData(List<GameObject> prefabs, RoomType type)
    {
        var list = new List<RoomPrefabData>();
        if (prefabs == null) return list;

        foreach (var pf in prefabs)
        {
            if (pf == null) continue;
            RoomPrefabData data = new RoomPrefabData();
            data.prefab = pf;
            data.type = type;
            list.Add(data);
        }
        return list;
    }

    // Cập nhật tất cả các room lists từ RoomDatabase theo theme hiện tại
    private void UpdateRoomListsForCurrentTheme()
    {
        if (roomDatabase == null || themeManager == null || roadDatabase == null)
        {
            Debug.LogWarning("[SimpleDungeonGenerator] RoomDatabase, RoadPrefabDatabase hoặc ThemeManager chưa gán — dùng lists cũ (fallback).");
            return;
        }

        ThemeType theme = themeManager.currentTheme;

        starterRooms = ConvertPrefabsToRoomPrefabData(roomDatabase.GetRooms(theme, RoomType.Starter), RoomType.Starter);
        normalRooms = ConvertPrefabsToRoomPrefabData(roomDatabase.GetRooms(theme, RoomType.Normal), RoomType.Normal);
        shopRooms = ConvertPrefabsToRoomPrefabData(roomDatabase.GetRooms(theme, RoomType.Shop), RoomType.Shop);
        blessingRooms = ConvertPrefabsToRoomPrefabData(roomDatabase.GetRooms(theme, RoomType.Blessing), RoomType.Blessing);
        specialRooms = ConvertPrefabsToRoomPrefabData(roomDatabase.GetRooms(theme, RoomType.Special), RoomType.Special);
        miniBossRooms = ConvertPrefabsToRoomPrefabData(roomDatabase.GetRooms(theme, RoomType.MiniBoss), RoomType.MiniBoss);
        finalRooms = ConvertPrefabsToRoomPrefabData(roomDatabase.GetRooms(theme, RoomType.Final), RoomType.Final);
        bossRooms = ConvertPrefabsToRoomPrefabData(roomDatabase.GetRooms(theme, RoomType.Boss), RoomType.Boss);
    }

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

    void CheckAndSpawnMissingRoads()
    {
        Debug.Log("<color=orange>🔍 Kiểm tra và spawn missing roads sau generation...</color>");
        var objectsCopy = new List<GameObject>(spawnedObjects);
        foreach (GameObject room in objectsCopy)
        {
            if (room == null || room.tag != "Room") continue;

            foreach (Transform child in room.GetComponentsInChildren<Transform>())
            {
                if (!child.CompareTag("BlockPoint")) continue;

                bool hasBlocker = HasChildWithTag(child, "Blocker");
                if (hasBlocker || blockedDoorPoints.Contains(child)) continue;

                // Ray hai hướng để tránh spawn road trùng
                RaycastHit2D hitForward = Physics2D.Raycast(child.position, child.right, 5f);
                RaycastHit2D hitBackward = Physics2D.Raycast(child.position, -child.right, 5f);
                if ((hitForward.collider != null && hitForward.collider.CompareTag("Road")) ||
                    (hitBackward.collider != null && hitBackward.collider.CompareTag("Road")))
                {
                    // Đã có road kết nối từ phía bên kia
                    continue;
                }

                // Tôn trọng maxRooms
                if (roomsSpawned >= maxRooms)
                {
                    BlockUnusedDoor(child);
                    continue;
                }

                GameObject road = Instantiate(roadPrefab, child.position, child.rotation, gridTransform);
                spawnedObjects.Add(road);

                Transform startPoint = null, endPoint = null;
                foreach (Transform t in road.GetComponentsInChildren<Transform>())
                {
                    if (t.CompareTag("StartPoint")) startPoint = t;
                    if (t.CompareTag("EndPoint")) endPoint = t;
                }

                if (startPoint == null || endPoint == null)
                {
                    Destroy(road);
                    continue;
                }

                Vector3 offset = startPoint.position - road.transform.position;
                road.transform.position -= offset;

                if (Vector3.Dot((endPoint.position - startPoint.position).normalized, child.right.normalized) < 0.95f)
                {
                    road.transform.rotation *= Quaternion.Euler(0, 0, 180);
                    offset = startPoint.position - road.transform.position;
                    road.transform.position -= offset;
                }

                if (endpointValidator != null)
                {
                    endpointValidator.CheckIntersectionAndReturnInactiveMarkers();
                    EndPointMarker marker = endPoint.GetComponent<EndPointMarker>();
                    if (marker != null && !marker.isValid)
                    {
                        Destroy(road);
                        BlockUnusedDoor(child);
                        continue;
                    }
                }

                Vector3 targetPos = endPoint.position;
                if (IsPositionOccupied(targetPos) || ContainsReserved(targetPos))
                {
                    Destroy(road);
                    BlockUnusedDoor(child);
                    continue;
                }

                TrySpawnRoomAt(endPoint);
            }
        }
    }

    IEnumerator GenerateUntilBossRoom()
    {
        int maxAttempts = 10; // Giới hạn lặp để tránh vòng lặp vô hạn nếu lỗi
        int attempts = 0;

        while (true)
        {
            attempts++;
            if (attempts > maxAttempts)
            {
                Debug.LogError("❌ Đạt giới hạn lặp, có thể có lỗi trong generate BossRoom.");
                break;
            }

            isGeneratingDone = false;
            ResetDungeon(); // FIX: Reset an toàn (dừng coroutine cũ và Destroy đúng cách)
            GenerateDungeon();

            // Chờ end-of-frame để các Destroy() xảy ra hoàn toàn
            yield return new WaitForEndOfFrame();

            if (bossSpawned)
            {
                Debug.Log("<color=green>✔️ Đã có BossRoom, kết thúc gen sau " + attempts + " lần thử.</color>");
                break;
            }
            else
            {
                Debug.LogWarning("❌ Không có BossRoom. Reset và thử lại...");
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    public void ResetDungeon()
    {
        // FIX: dừng toàn bộ coroutine cũ để tránh race conditions
        StopAllCoroutines();

        foreach (var obj in spawnedObjects)
        {
            if (obj != null)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying) DestroyImmediate(obj);
                else Destroy(obj);
#else
                Destroy(obj);
#endif
            }
        }

        spawnedObjects.Clear();
        doorQueue.Clear();
        occupiedPositions.Clear();
        blockedDoorPoints.Clear();
        reservedPositions.Clear();
        roomsSpawned = 0;
        shopSpawned = false;
        blessingSpawned = false;
        specialSpawned = false;
        miniBossSpawned = false;
        bossSpawned = false;
        generationComplete = false;

        EndPointMarker.AllMarkers.Clear();

        // Xoá rác còn sót trong scene
        GameObject[] leftoverRooms = GameObject.FindGameObjectsWithTag("Room");
        foreach (var room in leftoverRooms)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) DestroyImmediate(room);
            else Destroy(room);
#else
            Destroy(room);
#endif
        }
        GameObject[] leftoverRoads = GameObject.FindGameObjectsWithTag("Road");
        foreach (var road in leftoverRoads)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) DestroyImmediate(road);
            else Destroy(road);
#else
            Destroy(road);
#endif
        }

        Debug.Log("<color=yellow>Đã reset dungeon.</color>");
    }

    void GenerateDungeon()
    {
        UpdateRoomListsForCurrentTheme();

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

            if (endpointValidator != null)
            {
                List<EndPointMarker> invalidMarkers = endpointValidator.CheckIntersectionAndReturnInactiveMarkers();
                EndPointMarker doorMarker = doorPoint.GetComponent<EndPointMarker>();
                if (doorMarker != null && !doorMarker.isValid)
                {
                    Debug.LogWarning("🚫 DoorPoint không hợp lệ, bỏ qua: " + doorPoint.name);
                    BlockUnusedDoor(doorPoint);
                    continue;
                }
            }

            // Spawn road với rotation của doorPoint
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
                Destroy(road);
                continue;
            }

            // Điều chỉnh offset để StartPoint trùng DoorPoint, và kiểm tra hướng
            Vector3 offset = startPoint.position - doorPoint.position;
            road.transform.position -= offset;

            // Kiểm tra nếu EndPoint hướng đúng (dùng Dot để kiểm tra hướng so với DoorPoint.right)
            if (Vector3.Dot((endPoint.position - startPoint.position).normalized, doorPoint.right.normalized) < 0.95f)
            {
                Debug.LogWarning("⚠️ Road bị ngược hướng tại DoorPoint: " + doorPoint.name);
                // Fix ngược: Lật rotation 180 độ nếu sai hướng
                road.transform.rotation *= Quaternion.Euler(0, 0, 180);
                // Cập nhật lại offset sau lật
                offset = startPoint.position - doorPoint.position;
                road.transform.position -= offset;
            }

            if (endpointValidator != null)
                endpointValidator.CheckIntersectionAndReturnInactiveMarkers();

            TrySpawnRoomAt(endPoint);

            yield return null; // Giảm tải bằng cách yield mỗi frame
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
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        yield return new WaitForSeconds(0.1f);

        Debug.Log("<color=orange>🧹 Đang kiểm tra & vô hiệu hóa các Road không dùng...</color>");
        CleanUpUnusedRoads();
        HandleBlockedInUseMarkers();

        CheckAndSpawnMissingRoads();

        // FIX: chỉ replace khi thực sự đã spawn boss và stage < 3
        if (generationComplete && themeManager != null && bossSpawned && themeManager.stageIndexInTheme < 3)
        {
            ReplaceBossWithFinalRoom();
        }

        isGeneratingDone = true;
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(UpdateRoadsWithThemeOptimized());
    }

    private void ReplaceBossWithFinalRoom()
    {
        if (finalRooms == null || finalRooms.Count == 0)
        {
            Debug.LogWarning("[Debug] finalRooms rỗng!");
            return;
        }

        GameObject bossRoomToReplace = null;
        int bossLayer = LayerMask.NameToLayer("BossRoom");
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null && obj.layer == bossLayer)
            {
                bossRoomToReplace = obj;
                break; // Chỉ thay phòng Boss đầu tiên
            }
        }

        if (bossRoomToReplace == null)
        {
            // Không cảnh báo ồn ào nữa: ở stage < 3 thường không có boss
            return;
        }

        RoomPrefabData finalRoomData = finalRooms[Random.Range(0, finalRooms.Count)];
        GameObject newFinalRoom = Instantiate(finalRoomData.prefab, bossRoomToReplace.transform.position, bossRoomToReplace.transform.rotation, gridTransform);

        RoomController newRoomController = newFinalRoom.AddComponent<RoomController>();
        newRoomController.roomData = finalRoomData;

        spawnedObjects.Add(newFinalRoom);
        occupiedPositions.Add(newFinalRoom.transform.position);
        spawnedObjects.Remove(bossRoomToReplace);
        Destroy(bossRoomToReplace);
        bossSpawned = false;
        Debug.Log("<color=green>🔄 Đã thay thế BossRoom bằng FinalRoom.</color>");
    }

    IEnumerator UpdateRoadsWithThemeOptimized()
    {
        if (roadDatabase == null || themeManager == null)
        {
            Debug.LogWarning("[SimpleDungeonGenerator] RoadPrefabDatabase hoặc ThemeManager chưa gán.");
            yield break;
        }

        ThemeType theme = themeManager.currentTheme;
        var objectsCopy = new List<GameObject>(spawnedObjects);
        List<GameObject> roadsToReplace = new List<GameObject>();

        // Thu thập tất cả road cần thay thế
        foreach (GameObject obj in objectsCopy)
        {
            if (obj != null && obj.tag == "Road")
                roadsToReplace.Add(obj);
        }

        const int maxReplacementsPerFrame = 5;
        for (int i = 0; i < roadsToReplace.Count; i += maxReplacementsPerFrame)
        {
            int endIndex = Mathf.Min(i + maxReplacementsPerFrame, roadsToReplace.Count);
            for (int j = i; j < endIndex; j++)
            {
                GameObject obj = roadsToReplace[j];
                if (obj == null) continue;

                Vector3 originalPosition = obj.transform.position;
                Quaternion originalRotation = obj.transform.rotation;

                Transform startPoint = null, endPoint = null;
                foreach (Transform t in obj.GetComponentsInChildren<Transform>())
                {
                    if (t.CompareTag("StartPoint")) startPoint = t;
                    if (t.CompareTag("EndPoint")) endPoint = t;
                }

                if (startPoint == null || endPoint == null)
                {
                    Destroy(obj);
                    continue;
                }

                GameObject newRoad = null;
                int snapped = SnapAngleToRightAngle(originalRotation.eulerAngles.z);
                if (snapped == 0)
                {
                    newRoad = Instantiate(roadDatabase.GetRoadPrefab(theme, RoadDirection.Right), originalPosition, originalRotation, gridTransform);
                }
                else if (snapped == 270)
                {
                    newRoad = Instantiate(roadDatabase.GetRoadPrefab(theme, RoadDirection.Down), originalPosition, originalRotation, gridTransform);
                }
                else if (snapped == 90)
                {
                    newRoad = Instantiate(roadDatabase.GetRoadPrefab(theme, RoadDirection.Up), originalPosition, originalRotation, gridTransform);
                }
                else if (snapped == 180)
                {
                    newRoad = Instantiate(roadDatabase.GetRoadPrefab(theme, RoadDirection.Left), originalPosition, originalRotation, gridTransform);
                }

                if (newRoad != null)
                {
                    spawnedObjects.Add(newRoad);
                    spawnedObjects.Remove(obj);
                    Destroy(obj);
                }
                else
                {
                    Debug.LogWarning($"Không tìm thấy prefab cho rotation {originalRotation.eulerAngles.z} (snapped {snapped}) trong theme {theme}");
                }
            }
            yield return null; // Chờ đến frame tiếp theo để giảm tải
        }
    }

    void CleanUpUnusedRoads()
    {
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
                    Transform startPoint = null;
                    foreach (Transform t in obj.GetComponentsInChildren<Transform>())
                    {
                        if (t.CompareTag("StartPoint")) { startPoint = t; break; }
                    }

                    if (startPoint != null)
                    {
                        RaycastHit2D hit = Physics2D.Raycast(startPoint.position, startPoint.right, 10f);
                        if (hit.collider != null && hit.collider.CompareTag("BlockPoint"))
                        {
                            Transform doorPoint = hit.collider.transform;
                            BlockUnusedDoor(doorPoint);
                        }
                    }

                    spawnedObjects.Remove(obj);
                    Destroy(obj); // Xóa road không dùng
                }
            }
        }
    }

    void TrySpawnRoomAt(Transform endPoint)
    {
        // FIX: Tôn trọng maxRooms ở mọi entry point, đặc biệt khi được gọi từ CheckAndSpawnMissingRoads
        if (roomsSpawned >= maxRooms)
        {
            Debug.Log("🚫 Đạt maxRooms, huỷ road và gắn blocker tại endpoint: " + endPoint.name);

            if (doorBlockerPrefab != null)
            {
                Instantiate(doorBlockerPrefab, endPoint.position, endPoint.rotation, gridTransform);
            }

            // Xoá road parent nếu có
            Transform road = endPoint.parent;
            if (road != null)
            {
                Destroy(road.gameObject);
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

            if (ContainsReserved(targetPos) || IsPositionOccupied(targetPos))
            {
                Destroy(newRoom);
                continue;
            }

            newRoom.transform.position = targetPos;
            AddReserved(targetPos);

            // Thêm RoomController và gán roomData
            RoomController roomController = newRoom.AddComponent<RoomController>();
            roomController.roomData = roomData;

            // Gán layer "BossRoom" nếu là bossRooms
            // Nếu là BossRoom
            if (roomList == bossRooms)
            {
                newRoom.layer = LayerMask.NameToLayer("BossRoom");
                bossSpawned = true;
                Debug.Log("<color=red>⚔️ Đã spawn BossRoom!</color>");
            }
            // Nếu là FinalRoom
            else if (roomList == finalRooms)
            {
                newRoom.layer = LayerMask.NameToLayer("FinalRoom");
                bossSpawned = true; // dùng lại flag này để biết "end room đã spawn"
                Debug.Log("<color=magenta>🏁 Đã spawn FinalRoom!</color>");
            }


            spawnedObjects.Add(newRoom);
            occupiedPositions.Add(newRoom.transform.position);

            roomsSpawned++;

            Debug.Log($"<color=cyan>Spawned Room {roomsSpawned}/{maxRooms}: {newRoom.name}</color>");

            // Kiểm tra và spawn Blocker tại các BlockPoint nếu đạt maxRooms
            foreach (Transform child in newRoom.GetComponentsInChildren<Transform>())
            {
                if (child.CompareTag("BlockPoint") && roomsSpawned >= maxRooms)
                {
                    if (!HasChildWithTag(child, "Blocker") && doorBlockerPrefab != null)
                    {
                        GameObject blocker = Instantiate(doorBlockerPrefab, child.position, child.rotation, child);
                        blocker.transform.SetParent(child);
                        spawnedObjects.Add(blocker);
                    }
                }
                else if (child.CompareTag("DoorPoint") && child != actualDoor)
                {
                    // Kiểm tra nếu đã có road kết nối (Raycast hai hướng)
                    RaycastHit2D hitForward = Physics2D.Raycast(child.position, child.right, 5f);
                    RaycastHit2D hitBackward = Physics2D.Raycast(child.position, -child.right, 5f);
                    if ((hitForward.collider != null && hitForward.collider.CompareTag("Road")) ||
                        (hitBackward.collider != null && hitBackward.collider.CompareTag("Road")))
                    {
                        continue; // Không enqueue nếu đã kết nối từ bên kia
                    }

                    if (roomsSpawned >= maxRooms)
                    {
                        if (!HasChildWithTag(child, "Blocker") && doorBlockerPrefab != null)
                        {
                            GameObject blocker = Instantiate(doorBlockerPrefab, child.position, child.rotation, child);
                            blocker.transform.SetParent(child);
                            spawnedObjects.Add(blocker);
                        }
                    }
                    else
                    {
                        doorQueue.Enqueue(child);
                    }
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
                Vector3 offset = startPoint.position - blockedDoor.position;
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
        float bossRoomThreshold = 0.8f;
        int minRoomsBeforeBoss = Mathf.FloorToInt(maxRooms * bossRoomThreshold);

        if (!bossSpawned && roomsSpawned >= minRoomsBeforeBoss)
        {
            if (themeManager != null && themeManager.stageIndexInTheme == 3)
            {
                return bossRooms; // Stage cuối → BossRoom
            }
            else
            {
                return finalRooms; // Stage < 3 → FinalRoom
            }
        }


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
        // FIX: tránh double-block
        if (blockedDoorPoints.Contains(doorPoint)) return;
        if (HasChildWithTag(doorPoint, "Blocker")) return;

        if (doorBlockerPrefab != null)
        {
            GameObject blocker = Instantiate(doorBlockerPrefab, doorPoint.position, doorPoint.rotation);
            blocker.transform.SetParent(doorPoint);
            spawnedObjects.Add(blocker);
        }

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
