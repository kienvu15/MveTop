using UnityEngine;

[System.Serializable]
public class RoomPrefabData
{
    public RoomType type;            // Loại phòng (Normal, Boss, Shop, Special, Start, End)
    public GameObject prefab;        // Prefab tương ứng với loại phòng
    [HideInInspector]
    public bool hasBeenUsed = false; // Đánh dấu prefab này đã được dùng (dành cho logic spawn 1 lần nếu cần)
}
