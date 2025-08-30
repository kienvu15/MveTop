using UnityEngine;

public class EntryFinalTrigger : MonoBehaviour
{
    public RoomFinalController roomController;
    public FinalRoomSpawnController spawner;
    public GridManager gridManager;
    private bool triggered = false;

    private void Awake()
    {
        if (roomController == null)
        {
            roomController = GetComponentInParent<RoomFinalController>();
        }

        if (spawner == null)
        {
            spawner = GetComponentInParent<FinalRoomSpawnController>();
        }

        if (roomController == null)
        {
            Debug.LogError("RoomController not found in parent objects.");
        }

        if (spawner == null)
        {
            Debug.LogError("RoomSpawnerController not found in parent objects.");
        }
    }

    public void OnPlayerEnterRoom()
    {
        if (gridManager != null)
            gridManager.gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            roomController.PlayerEntered();  // Giao việc cho RoomController
            Destroy(gameObject); // Mỗi trigger dùng 1 lần
            gridManager.enabled = true; // Bật GridManager khi Player vào phòng
            OnPlayerEnterRoom();
        }
    }
}
