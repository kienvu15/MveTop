using System.Collections.Generic;
using UnityEngine;

public class RoomInstance
{
    public GameObject roomObject;                  // Phòng thật trong scene
    public List<DoorNode> doors = new List<DoorNode>();

    public RoomInstance(GameObject roomGO)
    {
        roomObject = roomGO;

        // Tìm tất cả DoorPoint
        Transform[] all = roomGO.GetComponentsInChildren<Transform>();
        foreach (var child in all)
        {
            if (child.CompareTag("DoorPoint"))
            {
                doors.Add(new DoorNode(child, this));
            }
        }
    }

    public int CountOpenDoors()
    {
        int count = 0;
        foreach (var door in doors)
        {
            if (!door.isConnected)
                count++;
        }
        return count;
    }
}
