using UnityEngine;

public class ShopItem : MonoBehaviour
{
    public string itemName = "Kiếm Lửa";
    public int price = 10;
    public bool isForSale = false;

    private bool playerInRange = false;
    private GameObject currentPlayer;

    private ShopItemWorldUI worldUI;

    public void Awake()
    {
        
    }

    private void Start()
    {
        worldUI = GetComponentInChildren<ShopItemWorldUI>();
        if (worldUI != null)
            worldUI.SetInfo(itemName, price);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if(isForSale == false)
            {
                worldUI.ShowWithoutText();

                // Gán chính nó cho PlayerInteract
                PlayerInteract interact = other.GetComponent<PlayerInteract>();
                if (interact != null)
                    interact.SetCurrentShopItem(this);
            }
            else if(isForSale == true)
            {
                worldUI.ShowWithText();

                // Gán chính nó cho PlayerInteract
                PlayerInteract interact = other.GetComponent<PlayerInteract>();
                if (interact != null)
                    interact.SetCurrentShopItem(this);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            worldUI?.Hide();

            // Clear reference trong Player
            PlayerInteract interact = other.GetComponent<PlayerInteract>();
            if (interact != null)
                interact.ClearCurrentShopItem(this);
        }
    }


    public void OnPurchased()
    {
        Debug.Log($"Đã mua {itemName} với giá {price} xu.");
        // Có thể thêm hiệu ứng, âm thanh, v.v ở đây
        isForSale = false; // Đánh dấu là đã bán
    }

}
