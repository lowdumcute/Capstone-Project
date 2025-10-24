using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance;

    [Header("References")]
    public Transform itemsParent; // Grid chứa các slot item
    [SerializeField] private InfoItemUI infoItemUI; // UI hiển thị chi tiết item (tùy chọn)

    private InventorySlot[] slots;
    private Inventory inventory;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        inventory = Inventory.instance;
        slots = itemsParent.GetComponentsInChildren<InventorySlot>(true);
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (inventory == null || slots == null) return;

        // ✅ Tạo danh sách hiển thị cho từng slot (spawn đúng bằng amount)
        List<Item> expandedItems = new List<Item>();

        foreach (var item in inventory.DataItem.Where(i => i.amount > 0))
        {
            if (item.isEquippable)
            {
                // Trang bị chỉ hiển thị 1 slot
                expandedItems.Add(item);
            }
            else
            {
                // Tiêu hao (Consumable): tạo slot cho từng đơn vị amount
                int count = item.amount;
                for (int j = 0; j < count; j++)
                {
                    // Tạo bản sao nhẹ cho UI hiển thị (không ảnh hưởng item gốc)
                    Item display = new Item
                    {
                        itemType = item.itemType,
                        isEquippable = item.isEquippable,
                        amount = 1, // mỗi slot là 1 đơn vị
                        BaseStatsItem = item.BaseStatsItem
                    };
                    expandedItems.Add(display);
                }
            }
        }

        // Giới hạn theo số lượng slot có sẵn
        expandedItems = expandedItems.Take(slots.Length).ToList();

        // ✅ Gán dữ liệu vào từng slot
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < expandedItems.Count)
                slots[i].AddItem(expandedItems[i]);
            else
                slots[i].ClearSlot();
        }
    }
}
