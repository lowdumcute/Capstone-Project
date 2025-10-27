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
        if (inventory == null) return;

        // ✅ Tạo danh sách item clone theo amount 
        // - Nếu là equippable thì chỉ 1 slot
        // - Nếu là consumable thì spawn đúng theo amount
        List<Item> expandedItems = new List<Item>();
        foreach (var item in inventory.DataItem.Where(i => i.amount > 0))
        {
            int count = item.isEquippable ? 1 : item.amount;
            for (int j = 0; j < count; j++)
            {
                expandedItems.Add(item);
            }
        }

        // Giới hạn theo số lượng slot có sẵn
        expandedItems = expandedItems.Take(slots.Length).ToList();

        // ✅ Hiển thị item lên slot
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < expandedItems.Count)
                slots[i].AddItem(expandedItems[i]);
            else
                slots[i].ClearSlot();
        }
    }
}
