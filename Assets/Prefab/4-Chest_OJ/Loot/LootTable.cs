using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Loot/LootTable")]
public class LootTable : ScriptableObject
{
    [Header("Cấu hình loot cho từng loại Chest")]
    public List<ChestLootGroup> chestLootGroups;

    public List<LootItem> GetLootItems(ChestType type)
    {
        var group = chestLootGroups.Find(g => g.chestType == type);
        return group != null ? group.lootItems : new List<LootItem>();
    }
}

[System.Serializable]
public class ChestLootGroup
{
    public ChestType chestType;          // Small / Medium / Big
    public List<LootItem> lootItems;     // Danh sách loot riêng cho chest đó
}
