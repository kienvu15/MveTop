using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopItemDatabase", menuName = "Shop/Item Database")]
public class ShopItemDatabase : ScriptableObject
{
    [Header("Danh sách prefab item")]
    public List<GameObject> shopItemPrefabs = new List<GameObject>();
}
