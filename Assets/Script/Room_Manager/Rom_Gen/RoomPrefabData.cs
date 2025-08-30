using UnityEngine;

[System.Serializable]
public class RoomPrefabData
{
    public RoomType type;            
    public GameObject prefab;        
    [HideInInspector]
    public bool hasBeenUsed = false;
}
