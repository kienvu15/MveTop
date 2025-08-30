using UnityEngine;

[System.Serializable]
public class CharacterData
{
    public string name;
    public Sprite avatar;
    public GameObject characterSelectionPrefab; // Hiển thị ở màn chọn
    public GameObject characterGameplayPrefab;  // Prefab chính dùng khi vào game
    public bool unlock;
}
