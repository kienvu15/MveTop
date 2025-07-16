using System.Collections.Generic;
using UnityEngine;

public class DungeonState
{
    public List<RoomInstance> spawnedRooms = new List<RoomInstance>();
    public List<DoorNode> openDoors = new List<DoorNode>();

    public int maxRooms = 10;
    public int roomsSpawned => spawnedRooms.Count;

    public void RegisterRoom(RoomInstance room)
    {
        spawnedRooms.Add(room);

        // Thêm các cửa chưa nối vào danh sách openDoors
        foreach (var door in room.doors)
        {
            if (!door.isConnected)
                openDoors.Add(door);
        }
    }

    public void MarkDoorConnected(DoorNode door)
    {
        door.isConnected = true;
        openDoors.Remove(door);
    }

    public bool CanContinue() => roomsSpawned < maxRooms && openDoors.Count > 0;
}
