using UnityEngine;
using System.Collections.Generic;

public class Chest : MonoBehaviour, IInteractable
{
    public bool isOpened { get; private set; }
    public string chestID { get; private set; }

    [Header("Chest Settings")]
    public LootTable lootTable;
    private ChestType chestType;

    public Sprite openSprite;
    public float dropRadius = 1.5f; // Điều chỉnh được

    void Start()
    {
        chestID ??= GlobalHelper.GenerateUniqueID(gameObject);
    }

    // Gọi khi spawn từ RoomSpawnerController
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
        var lootItems = lootTable.GetLootItems(chestType);

        // Tạo sẵn offset để spawn, ví dụ tối đa 20 vật phẩm
        List<Vector3> spawnOffsets = GenerateSpawnOffsets(20, dropRadius);
        int spawnIndex = 0;

        foreach (var item in lootItems)
        {
            if (Random.value <= item.dropChance)
            {
                int amount = Random.Range(item.minQuantity, item.maxQuantity + 1);

                for (int i = 0; i < amount; i++)
                {
                    if (spawnIndex >= spawnOffsets.Count) // hết slot offset
                        break;

                    Vector3 spawnPos = transform.position + spawnOffsets[spawnIndex];
                    spawnIndex++;

                    GameObject obj = Instantiate(item.itemPrefab, spawnPos, Quaternion.identity);

                    // Nếu item có hiệu ứng bounce thì gọi
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
            float distance = Random.Range(radius * 0.5f, radius); // ngẫu nhiên trong bán kính
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;

            // Jitter nhẹ để không quá đều
            offset += (Vector3)(Random.insideUnitCircle * 0.2f);

            offsets.Add(offset);
        }

        return offsets;
    }
}
