using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("Pickup Settings")]
    public float pickupRadius = 2f;
    public KeyCode pickupKey = KeyCode.B;

    [Header("Shop Settings")]
    private ShopItem currentNearbyShopItem;
    public KeyCode buyKey = KeyCode.X;

    private InventoryController inventoryController;

    void Start()
    {
        inventoryController = FindFirstObjectByType<InventoryController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(pickupKey))
        {
            TryPickupClosestItem();
        }

        if (Input.GetKeyDown(buyKey))
        {
            TryBuyCurrentShopItem();
        }
    }

    private void TryPickupClosestItem()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, pickupRadius);
        float closestDistance = float.MaxValue;
        Collider2D closestItem = null;

        foreach (Collider2D col in hits)
        {
            if (col.CompareTag("Item"))
            {
                float dist = Vector2.Distance(transform.position, col.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestItem = col;
                }
            }
        }

        if (closestItem != null)
        {
            Item item = closestItem.GetComponent<Item>();
            ShopItem items = closestItem.GetComponent<ShopItem>();

                if (item != null && items.isForSale == false) // Chỉ nhặt nếu KHÔNG phải item từ shop
                {
                    bool added = inventoryController.AddItem(closestItem.gameObject);
                    if (added)
                    {
                        item.Pickup();
                        Destroy(closestItem.gameObject);
                    }
                    else
                    {
                        Debug.Log("Inventory is full, cannot pick up item.");
                    }
                }

            
        }
        else
        {
            Debug.Log("No item nearby to pick up.");
        }
    }

    public void SetCurrentShopItem(ShopItem shopItem)
    {
        currentNearbyShopItem = shopItem;
    }

    public void ClearCurrentShopItem(ShopItem shopItem)
    {
        // Đảm bảo không bị ghi nhầm bởi 2 shop item chồng nhau
        if (currentNearbyShopItem == shopItem)
            currentNearbyShopItem = null;
    }

    private void TryBuyCurrentShopItem()
    {
        if (currentNearbyShopItem == null || !currentNearbyShopItem.isForSale)
        {
            Debug.Log("Không có item shop hợp lệ để mua.");
            return;
        }

        int price = currentNearbyShopItem.price;

        if (CoinManager.Instance.coinCount >= price)
        {
            CoinManager.Instance.coinCount -= price;
            CoinManager.Instance.UpdateCoinUI();

            bool added = inventoryController.AddItem(currentNearbyShopItem.gameObject);
            if (added)
            {
                currentNearbyShopItem.OnPurchased();
                Destroy(currentNearbyShopItem.gameObject);
            }
            else
            {
                Debug.Log("Inventory đầy, không thể thêm item.");
            }

            

        }
        else
        {
            Debug.Log("Không đủ xu để mua.");
        }
    }


    // Hiển thị phạm vi nhặt đồ trong Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}
