using UnityEngine;

public class SpecialEquipSlot : Slot
{
    // Đây chỉ chứa item đặc biệt
    public bool CanAcceptSpecialItem(Item item)
    {
        // Giả sử item có loại đặc biệt (ví dụ ID hoặc flag)
        return item.isSpecialItem;
    }
}
