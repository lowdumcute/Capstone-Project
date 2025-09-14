using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public List<EquipmentSlot> equipmentSlots;
    public static EquipmentManager instance;

    void Start()
    {
        instance = this;
    }

    public void Equip(BaseItem item)
    {
        foreach (var slot in equipmentSlots)
        {
            if (slot.slotType == item.itemType)
            {
                // Nếu đã có item trang bị trong slot, trả nó về inventory
                if (slot.currentItem != null)
                {
                    Inventory.instance.Add(new Item 
                    { 
                        isEquippable = true, 
                        amount = 1, 
                        BaseStatsItem = slot.currentItem 
                    });
                }

                // Trang bị item mới
                slot.currentItem = item;
                slot.CheckItemEquip();   // ✅ cập nhật lại UI icon

                Debug.Log("Equipped: " + item.itemName);
                return;
            }
        }
        Debug.LogWarning("No suitable slot found for item: " + item.itemName);
    }
}
