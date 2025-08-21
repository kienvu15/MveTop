using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopItemDatabase", menuName = "Shop/Item Database")]
public class ShopItemDatabase : ScriptableObject
{
    [Header("Danh s�ch prefab item")]
    public List<GameObject> shopItemPrefabs = new List<GameObject>();
}
