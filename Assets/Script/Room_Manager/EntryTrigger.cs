using UnityEngine;

public class EntryTrigger : MonoBehaviour
{
    public RoomController roomController;
    public RoomSpawnerController spawner;

    private void Awake()
    {
        if (roomController == null)
        {
            roomController = GetComponentInParent<RoomController>();
        }
        if (roomController == null)
        {
            Debug.LogError("RoomController not found in parent objects.");
        }
    }

    private void Start()
    {
        if (spawner == null)
        {
            spawner = FindFirstObjectByType<RoomSpawnerController>();
        }
        if (spawner == null)
        {
            Debug.LogError("RoomSpawnerController not found in the scene.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            roomController.PlayerEntered();
            spawner.StartRoom();
            Destroy(gameObject);
        }
    }
}
