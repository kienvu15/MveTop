using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    Transform originalParent;
    CanvasGroup canvasGroup;

    public float minDropDistance = 2f;
    public float maxDropDistance = 3f;
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {

    }

    public void OnBeginDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(transform.root);
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }
    public void OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        Slot dropSlot = eventData.pointerEnter?.GetComponent<Slot>();
        if (dropSlot == null)
        {
            GameObject dropItem = eventData.pointerEnter;
            if (dropItem != null)
            {
                dropSlot = dropItem.GetComponentInParent<Slot>();
            }
        }

        Slot originalSlot = originalParent.GetComponent<Slot>();

        if (dropSlot != null && dropSlot != originalSlot)
        {
            Item draggedItem = GetComponent<Item>();

            // Chặn item thường vào SpecialEquipSlot
            SpecialEquipSlot specialSlot = dropSlot.GetComponent<SpecialEquipSlot>();
            if (specialSlot != null)
            {
                if (draggedItem == null || !draggedItem.isSpecialItem)
                {
                    Debug.Log("Không thể đặt item thường vào ô đặc biệt!");
                    transform.SetParent(originalParent);
                    GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    return;
                }
            }

            // Chặn item đặc biệt vào EquipSlot thường
            EquipSlot equipSlot = dropSlot.GetComponent<EquipSlot>();
            if (equipSlot != null)
            {
                if (draggedItem != null && draggedItem.isSpecialItem)
                {
                    Debug.Log("Item đặc biệt chỉ được trang bị vào ô đặc biệt!");
                    transform.SetParent(originalParent);
                    GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    return;
                }
            }

            // Nếu slot đích đã có item → đổi chỗ
            if (dropSlot.currentItem != null)
            {
                dropSlot.currentItem.transform.SetParent(originalSlot.transform);
                dropSlot.currentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                originalSlot.currentItem = dropSlot.currentItem;

                // Đánh dấu item cũ trong slot đích là đã bị gỡ trang bị
                Item itemInSlot = dropSlot.currentItem.GetComponent<Item>();
                if (itemInSlot != null)
                {
                    itemInSlot.isEquipped = false;
                }
            }
            else
            {
                originalSlot.currentItem = null;
            }

            // Đưa item hiện tại vào slot đích
            transform.SetParent(dropSlot.transform);
            dropSlot.currentItem = gameObject;

            // Cập nhật trạng thái isEquipped
            if (dropSlot.GetComponent<EquipSlot>() != null || dropSlot.GetComponent<SpecialEquipSlot>() != null)
            {
                if (draggedItem != null)
                {
                    draggedItem.isEquipped = true;
                }
            }
            else
            {
                if (draggedItem != null)
                {
                    draggedItem.isEquipped = false;
                }
            }
        }
        else
        {
            // Nếu thả ra ngoài slot hoặc vào chính slot gốc
            if (!IsWithinInventory(eventData.position))
            {
                DropItems(originalSlot);
                return;
            }
            else
            {
                transform.SetParent(originalParent);
            }
        }

        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    bool IsWithinInventory(Vector2 mousePosition)
    {
        RectTransform inventoryRect = originalParent.parent.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(inventoryRect, mousePosition);
    }

    void DropItems(Slot originalSlot)
    {
        originalSlot.currentItem = null;
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if(playerTransform == null)
        {
            Debug.LogError("Missing Player");
            return;
        }

        Vector2 dropOffset = Random.insideUnitCircle.normalized * Random.Range(minDropDistance, maxDropDistance);
        Vector2 dropPosition = (Vector2)playerTransform.position + dropOffset;
        GameObject dropItems = Instantiate(gameObject, dropPosition, Quaternion.identity);
        dropItems.GetComponent<BounceEffect>().StartBounce();
        Destroy(gameObject);
    }
}
