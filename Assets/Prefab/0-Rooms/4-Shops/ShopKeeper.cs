using System.Collections.Generic;
using UnityEngine;
using TMPro; // Nếu dùng TextMeshPro

public class ShopKeeper : MonoBehaviour
{
    [Header("Database chứa item shop")]
    public ShopItemDatabase itemDatabase;

    [Header("Vị trí spawn các item shop")]
    public Transform[] itemSpawnPoints;

    [Header("Phím để làm mới shop")]
    public Canvas speak;
    public TextMeshProUGUI priceText;
    public int costRefresh = 5;
    public KeyCode refreshKey = KeyCode.E;

    private List<GameObject> currentSpawnedItems = new List<GameObject>();

    void Start()
    {
        SpawnNewShopItems();
        speak.enabled = false;

        
    }

    void Update()
    {
        if (Input.GetKeyDown(refreshKey))
        {
            RefeshItem();
        }

        SetInfo();
    }

    public void SetInfo()
    {
        priceText.text = "Giá: " + costRefresh + " xu";
    }

    private void RefeshItem()
    {
        int coin = costRefresh;
        if (CoinManager.Instance.coinCount >= costRefresh)
        {
            CoinManager.Instance.coinCount -= costRefresh;
            CoinManager.Instance.UpdateCoinUI();
            costRefresh += 5;
            RefreshShop();
        }
        else
        {
            Debug.Log("Không đủ xu để mua.");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            // Khi người chơi va chạm với ShopKeeper, có thể hiển thị thông báo hoặc làm gì đó
            Debug.Log("Player entered shop area.");
            speak.enabled = true; // Hiển thị canvas thông báo

            if (Input.GetKeyDown(refreshKey))
            {
                RefeshItem();
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            // Khi người chơi vẫn ở trong khu vực ShopKeeper, có thể hiển thị thông báo
            Debug.Log("Player is in shop area.");
            speak.enabled = true; // Hiển thị canvas thông báo
            if (Input.GetKeyDown(refreshKey))
            {
                RefeshItem();
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            // Khi người chơi rời khỏi khu vực ShopKeeper, có thể ẩn thông báo
            Debug.Log("Player exited shop area.");
            speak.enabled = false; // Ẩn canvas thông báo
        }
    }

    public void RefreshShop()
    {
        foreach (GameObject item in currentSpawnedItems)
        {
            if (item != null)
                Destroy(item);
        }
        currentSpawnedItems.Clear();
        SpawnNewShopItems();
    }

    void SpawnNewShopItems()
    {
        if (itemDatabase == null || itemDatabase.shopItemPrefabs.Count == 0)
        {
            Debug.LogWarning("Item Database rỗng hoặc chưa được gán!");
            return;
        }

        // Tạo bản sao danh sách prefab từ database
        List<GameObject> tempPool = new List<GameObject>(itemDatabase.shopItemPrefabs);

        for (int i = 0; i < itemSpawnPoints.Length; i++)
        {
            if (tempPool.Count == 0)
            {
                Debug.LogWarning("Không đủ prefab khác nhau để spawn cho tất cả slot.");
                break;
            }

            int randIndex = Random.Range(0, tempPool.Count);
            GameObject randomPrefab = tempPool[randIndex];
            tempPool.RemoveAt(randIndex);

            Transform spawnPoint = itemSpawnPoints[i];
            GameObject spawnedItem = Instantiate(randomPrefab, spawnPoint.position, Quaternion.identity, spawnPoint);
            currentSpawnedItems.Add(spawnedItem);

            ShopItem shopItem = spawnedItem.GetComponent<ShopItem>();
            if (shopItem != null)
                shopItem.isForSale = true;
        }
    }
}
