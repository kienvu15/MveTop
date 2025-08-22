using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    public bool isOpened { get; private set; }
    public string chestID { get; private set; }

    [Header("Chest Settings")]
    public LootTable lootTable;
    public ShopItemDatabase shopItemDatabase;
    [SerializeField, ReadOnly(true)] public ChestType chestType;

    [Header("Big Chest Settings")]
    [Range(0f, 1f)] public float shopItemDropChance = 0.7f; // tỉ lệ rớt item shop

    public Sprite openSprite;
    public float dropRadius = 1.5f;

    void Start()
    {
        chestID ??= GlobalHelper.GenerateUniqueID(gameObject);
    }

    public void Init(RoomClass roomClass)
    {
        chestType = MapRoomClassToChestType(roomClass);
    }

    private ChestType MapRoomClassToChestType(RoomClass roomClass)
    {
        switch (roomClass)
        {
            case RoomClass.Small: return ChestType.Small;
            case RoomClass.Medium: return ChestType.Medium;
            case RoomClass.Big: return ChestType.Big;
            default: return ChestType.Small;
        }
    }

    public void Interact()
    {
        if (!CanInteract())
            return;

        OpenChest();
    }

    public bool CanInteract() => !isOpened;

    private void OpenChest()
    {
        SetOpened(true);
        DropLoot();
    }

    public void SetOpened(bool opened)
    {
        isOpened = opened;
        if (opened)
        {
            GetComponent<SpriteRenderer>().sprite = openSprite;
        }
    }

    public void DropLoot()
    {
        List<Vector3> spawnOffsets = GenerateSpawnOffsets(20, dropRadius);
        int spawnIndex = 0;

        // Nếu là Big chest: có tỉ lệ rơi item từ ShopItemDatabase
        if (chestType == ChestType.Big && shopItemDatabase != null && shopItemDatabase.shopItemPrefabs.Count > 0)
        {
            if (Random.value <= shopItemDropChance)
            {
                GameObject prefab = shopItemDatabase.shopItemPrefabs[Random.Range(0, shopItemDatabase.shopItemPrefabs.Count)];
                Vector3 spawnPos = transform.position + spawnOffsets[spawnIndex];
                spawnIndex++;

                GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity);
                Debug.Log("Spawned shop item: " + prefab.name + " at " + spawnPos);

                if (obj.TryGetComponent(out BounceEffect bounce))
                    bounce.StartBounce();
            }

        }

        // Loot phụ từ LootTable
        var lootItems = lootTable.GetLootItems(chestType);

        foreach (var item in lootItems)
        {
            if (Random.value <= item.dropChance)
            {
                int amount = Random.Range(item.minQuantity, item.maxQuantity + 1);

                for (int i = 0; i < amount; i++)
                {
                    if (spawnIndex >= spawnOffsets.Count)
                        break;

                    Vector3 spawnPos = transform.position + spawnOffsets[spawnIndex];
                    spawnIndex++;

                    GameObject obj = Instantiate(item.itemPrefab, spawnPos, Quaternion.identity);

                    if (obj.TryGetComponent(out BounceEffect bounce))
                        bounce.StartBounce();
                }
            }
        }
    }

    private List<Vector3> GenerateSpawnOffsets(int count, float radius)
    {
        List<Vector3> offsets = new();

        for (int i = 0; i < count; i++)
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float distance = Random.Range(radius * 0.5f, radius);
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
            offset += (Vector3)(Random.insideUnitCircle * 0.2f);
            offsets.Add(offset);
        }

        return offsets;
    }
}
