using UnityEngine;

public class EntryBossTrigger : MonoBehaviour
{
    public RoomBossController roomController;
    public BossRoomSpawnerController spawner;

    private bool triggered = false;

    private void Awake()
    {
        if (roomController == null)
        {
            roomController = GetComponentInParent<RoomBossController>();
        }

        if (spawner == null)
        {
            spawner = GetComponentInParent<BossRoomSpawnerController>();
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            roomController.PlayerEntered();  // Giao việc cho RoomController
            Destroy(gameObject); // Mỗi trigger dùng 1 lần
        }
    }

}
