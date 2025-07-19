using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance { get; private set; }

    [Header("Inventory Settings")]
    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int inventorySlotCount = 16;

    [Header("Equip Settings")]
    public GameObject equipPanel;
    public GameObject equipSlotPrefab;
    public GameObject specialEquipSlotPrefab;
    public int equipSlotCount = 5;

    [Header("Item Settings")]
    public GameObject[] itemPrefab;  // Dùng để test sẵn item

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        SpawnEquipSlots();
        SpawnInventorySlots();
    }

    void SpawnEquipSlots()
    {
        for (int i = 0; i < equipSlotCount; i++)
        {
            GameObject slot;
            if (i == 0)   // Slot đầu tiên là Special Slot (ô đỏ)
            {
                slot = Instantiate(specialEquipSlotPrefab, equipPanel.transform);
            }
            else
            {
                slot = Instantiate(equipSlotPrefab, equipPanel.transform);
            }

            slot.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
    }

    void SpawnInventorySlots()
    {
        for (int i = 0; i < inventorySlotCount; i++)
        {
            Slot slot = Instantiate(slotPrefab, inventoryPanel.transform).GetComponent<Slot>();
            slot.currentItem = null;   // Khởi tạo slot trống
        }

        // (Tùy chọn) Nếu muốn spawn sẵn item để test:
        for (int i = 0; i < itemPrefab.Length && i < inventorySlotCount; i++)
        {
            Transform slotTransform = inventoryPanel.transform.GetChild(i);
            Slot slot = slotTransform.GetComponent<Slot>();
            GameObject item = Instantiate(itemPrefab[i], slotTransform);
            item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            slot.currentItem = item;
        }
    }

    /// Gọi khi nhặt item
    public bool AddItem(GameObject itemPrefab)
    {
        bool added = AddItemToEquip(itemPrefab);

        if (!added)
        {
            added = AddItemToInventory(itemPrefab);
        }

        return added;
    }


    public bool AddItemToEquip(GameObject itemPrefab)
    {
        Item itemComponent = itemPrefab.GetComponent<Item>();

        foreach (Transform slotTransform in equipPanel.transform)
        {
            SpecialEquipSlot specialSlot = slotTransform.GetComponent<SpecialEquipSlot>();
            if (specialSlot != null && itemComponent.isSpecialItem)
            {
                if (specialSlot.currentItem == null)
                {
                    GameObject newItem = Instantiate(itemPrefab, slotTransform);
                    newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    specialSlot.currentItem = newItem;

                    Item itemScript = newItem.GetComponent<Item>();
                    itemScript.isEquipped = true;

                    return true;
                }
            }

            EquipSlot equipSlot = slotTransform.GetComponent<EquipSlot>();
            if (equipSlot != null && equipSlot.currentItem == null)
            {
                GameObject newItem = Instantiate(itemPrefab, slotTransform);
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                equipSlot.currentItem = newItem;

                Item itemScript = newItem.GetComponent<Item>();
                itemScript.isEquipped = true;

                return true;
            }
        }

        return false;  // Không còn slot trống
    }


    public bool AddItemToInventory(GameObject itemPrefab)
    {
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot.currentItem == null)
            {
                GameObject newItem = Instantiate(itemPrefab, slotTransform);
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = newItem;

                Item itemScript = newItem.GetComponent<Item>();
                itemScript.isEquipped = false;

                return true;
            }
        }

        return false;  // Inventory đầy
    }

}
