using UnityEngine;

public class EntryTrigger : MonoBehaviour
{
    public RoomController roomController;
    public RoomSpawnerController spawner;

    private bool triggered = false;

    private void Awake()
    {
        if (roomController == null)
        {
            roomController = GetComponentInParent<RoomController>();
        }

        if (spawner == null)
        {
            spawner = GetComponentInParent<RoomSpawnerController>();
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
