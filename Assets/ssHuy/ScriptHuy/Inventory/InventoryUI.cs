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

        List<Item> displayItems = new List<Item>();

        foreach (var item in inventory.DataItem)
        {
            if (item.amount <= 0)
                continue;

            int showAmount = item.isEquippable
                ? Mathf.Max(item.amount - 1, 0)   // nếu trang bị thì trừ 1
                : item.amount;

            // ⚙️ Nếu sau khi trừ còn 0 thì bỏ qua (đã trang bị hết)
            if (showAmount <= 0)
                continue;

            // Tạo bản sao để hiển thị
            Item display = new Item
            {
                itemType = item.itemType,
                isEquippable = item.isEquippable,
                amount = showAmount,
                BaseStatsItem = item.BaseStatsItem
            };

            displayItems.Add(display);
        }

        // Giới hạn theo số lượng slot
        displayItems = displayItems.Take(slots.Length).ToList();

        // ✅ Gán dữ liệu vào từng slot
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < displayItems.Count)
                slots[i].AddItem(displayItems[i]);
            else
                slots[i].ClearSlot();
        }
    }
}
