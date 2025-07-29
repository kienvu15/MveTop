using System.Collections.Generic;
using UnityEngine;

public class ShopKeeper : MonoBehaviour
{
    [Header("Danh sách prefab item có thể bán")]
    public List<GameObject> shopItemPrefabs;

    [Header("Vị trí spawn các item shop")]
    public Transform[] itemSpawnPoints;

    [Header("Phím để làm mới shop")]
    public KeyCode refreshKey = KeyCode.E;

    private List<GameObject> currentSpawnedItems = new List<GameObject>();

    void Start()
    {
        SpawnNewShopItems();
    }

    void Update()
    {
        if (Input.GetKeyDown(refreshKey))
        {
            RefreshShop();
        }
    }

    public void RefreshShop()
    {
        // Xoá item cũ nếu có
        foreach (GameObject item in currentSpawnedItems)
        {
            if (item != null)
                Destroy(item);
        }
        currentSpawnedItems.Clear();

        // Gọi spawn mới
        SpawnNewShopItems();
    }

    void SpawnNewShopItems()
    {
        // Tạo bản sao danh sách prefab để loại bỏ dần
        List<GameObject> tempPool = new List<GameObject>(shopItemPrefabs);

        for (int i = 0; i < itemSpawnPoints.Length; i++)
        {
            if (tempPool.Count == 0)
            {
                Debug.LogWarning("Không đủ prefab khác nhau để spawn cho tất cả slot.");
                break;
            }

            // Random 1 prefab rồi xoá khỏi pool
            int randIndex = Random.Range(0, tempPool.Count);
            GameObject randomPrefab = tempPool[randIndex];
            tempPool.RemoveAt(randIndex);

            Transform spawnPoint = itemSpawnPoints[i];
            GameObject spawnedItem = Instantiate(randomPrefab, spawnPoint.position, Quaternion.identity);
            currentSpawnedItems.Add(spawnedItem);

            ShopItem shopItem = spawnedItem.GetComponent<ShopItem>();
            if (shopItem != null)
            {
                shopItem.isForSale = true;
            }

        }
    }

}
