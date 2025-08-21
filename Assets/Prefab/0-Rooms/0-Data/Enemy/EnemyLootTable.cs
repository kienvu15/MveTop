using UnityEngine;

[System.Serializable]
public class DropItem
{
    public GameObject prefab;        // Prefab của vật phẩm
    public float dropWeight = 1f;    // Tỉ lệ tương đối
}

[CreateAssetMenu(fileName = "EnemyLootData", menuName = "Loot/EnemyLootTable")]
public class EnemyLootTable : ScriptableObject
{
    public DropItem[] dropItems;     // Danh sách vật phẩm có thể rớt
    public int maxDrop = 3;
}
