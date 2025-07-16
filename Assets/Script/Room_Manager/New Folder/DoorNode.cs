using UnityEngine;

public class DoorNode
{
    public Transform doorTransform;   // Cửa thật trong scene
    public bool isConnected;          // Đã nối chưa
    public RoomInstance parentRoom;   // Phòng sở hữu cửa này

    public Vector3 GetPosition() => doorTransform.position;
    public Vector3 GetDirection() => doorTransform.right;  // Hoặc forward, tuỳ prefab bạn

    public DoorNode(Transform transform, RoomInstance parent)
    {
        doorTransform = transform;
        parentRoom = parent;
        isConnected = false;
    }
}
