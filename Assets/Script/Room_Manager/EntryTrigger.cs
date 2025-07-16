using UnityEngine;

public class EntryTrigger : MonoBehaviour
{
    public RoomController roomController;

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            roomController.PlayerEntered();
        }
    }
}
