using UnityEngine;

[System.Serializable]
public class LootItem
{
    public GameObject itemPrefab;
    [Range(0f, 1f)]
    public float dropChance; // Tỉ lệ rớt (0.5 = 50%)
    public int minQuantity = 1;
    public int maxQuantity = 1;
}
