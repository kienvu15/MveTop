using UnityEngine;
using System.Collections.Generic;

public class Chest : MonoBehaviour, IInteractable
{
    public bool isOpened { get; private set; }
    public string chestID { get; private set; }

    [Header("Chest Settings")]
    public ChestType chestType;
    public Sprite openSprite;
    public LootTable lootTable;
    public float dropRadius = 1.5f; // Điều chỉnh được

    void Start()
    {
        chestID ??= GlobalHelper.GenerateUniqueID(gameObject);
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

    private void DropLoot()
    {
        if (lootTable == null)
        {
            Debug.LogWarning("No loot table assigned.");
            return;
        }

        List<Vector3> spawnOffsets = GenerateSpawnOffsets(15, dropRadius); // tối đa 15 vật phẩm/ngẫu nhiên

        int spawnIndex = 0;

        foreach (var loot in lootTable.lootItems)
        {
            if (Random.value < loot.dropChance)
            {
                int quantity = Random.Range(loot.minQuantity, loot.maxQuantity + 1);

                for (int i = 0; i < quantity; i++)
                {
                    if (spawnIndex >= spawnOffsets.Count)
                        break;

                    Vector3 spawnPos = transform.position + spawnOffsets[spawnIndex];
                    spawnIndex++;

                    GameObject item = Instantiate(loot.itemPrefab, spawnPos, Quaternion.identity);

                    if (item.TryGetComponent(out BounceEffect bounce))
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
