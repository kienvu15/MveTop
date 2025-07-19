using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private InventoryController inventoryController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventoryController = FindFirstObjectByType<InventoryController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            Item item = collision.GetComponent<Item>();
            if (item != null)
            {
                bool itemAdd = inventoryController.AddItem(collision.gameObject);
                if (itemAdd)
                {
                    item.Pickup();
                    Destroy(collision.gameObject);
                }
                else
                {
                    Debug.Log("Inventory is full, cannot add item.");
                }
            }
        }
    }
}
