using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [Header("Room Systems")]
    public RoomSpawnerController spawner;
    [SerializeField] private RoomRewardSpawner rewardSpawner;

    [Header("Room Components")]
    public PolygonCollider2D confinerAreaSoft;   // Vùng rộng hơn
    public PolygonCollider2D confinerAreaHard;   // Vùng phòng thật sự
    public List<GameObject> doors;  // Nhiều cửa


    [Header("Camera")]
    public CinemachineCamera virtualCamera;

    private bool roomActivated = false;

    private void Awake()
    {
        spawner = GetComponent<RoomSpawnerController>();

        if (virtualCamera == null)
        {
            virtualCamera = FindFirstObjectByType<CinemachineCamera>();
        }

        Transform doorsRoot = transform.Find("Doors");
        doors = new List<GameObject>();
        foreach (Transform child in doorsRoot)
        {
            doors.Add(child.gameObject);
            Debug.Log($"Đã thêm cửa: {child.name}");
        }

        rewardSpawner = GetComponent<RoomRewardSpawner>();

        // Tìm object cha chứa cả 2 confiner
        Transform confinerParent = transform.Find("ConfinerArea");

        if (confinerParent != null)
        {
            Transform soft = confinerParent.Find("ConfinerArea_Soft");
            Transform hard = confinerParent.Find("ConfinerArea_Hard");

            if (soft != null)
            {
                confinerAreaSoft = soft.GetComponent<PolygonCollider2D>();
            }
            if (hard != null)
            {
                confinerAreaHard = hard.GetComponent<PolygonCollider2D>();
            }
        }

        OpenDoors();
    }

    private bool roomStarted = false;

    public void PlayerEntered()
    {
        if (roomStarted) return;

        roomStarted = true;

        // Đóng cửa nếu cần
        CloseDoors();

        // Bắt đầu spawn
        if (spawner != null)
        {
            spawner.StartRoom();
        }
        else
        {
            Debug.LogWarning("Spawner không được gán trong RoomController.");
        }
    }



    private IEnumerator SoftConfinerTransition()
    {
        SetCameraConfiner(confinerAreaSoft);

        yield return new WaitForSeconds(3f);   // Cho camera di chuyển vào mượt mà

        SetCameraConfiner(confinerAreaHard);     // Sau đó khóa chặt camera
    }

    private void SetCameraConfiner(PolygonCollider2D area)
    {
        var confiner = virtualCamera.GetComponent<CinemachineConfiner2D>();
        confiner.BoundingShape2D = area;
        confiner.InvalidateBoundingShapeCache();
    }

    private void CloseDoors()
    {
        foreach (var door in doors)
        {
            door.SetActive(true);  // Đóng từng cửa (bật collider + sprite)
        }
    }

    public void OpenDoors()
    {
        foreach (var door in doors)
        {
            door.SetActive(false);  // Mở từng cửa (tắt collider + sprite)
        }
    }

    public void OnRoomCleared()
    {
        OpenDoors();
        rewardSpawner?.SpawnReward();
    }
}
