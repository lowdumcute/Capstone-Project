using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance;
    public Transform itemsParent; // grid chứa các slot

    Inventory inventory;
    InventorySlot[] slots;
    [SerializeField] InfoItemUI infoItemUI;


    private ItemType? currentFilter = null; // để nhớ filter hiện tại
    void Awake()
    {
        instance = this;
        
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        inventory = Inventory.instance;
        slots = itemsParent.GetComponentsInChildren<InventorySlot>();

        // load lần đầu: hiển thị tất cả
        UpdateUI();
    }

    void Update()
    {
        StatsUI.Instance.UpdateStatsUI();
    }

    /// <summary>
    /// Cập nhật UI với filter
    /// </summary>
    public void UpdateUI(ItemType? filter = null)
    {
        currentFilter = filter;

        // Clear hết slot trước
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].ClearSlot();
            slots[i].button.onClick.RemoveAllListeners();
        }

        int slotIndex = 0;

        for (int i = 0; i < inventory.DataItem.Count; i++)
        {
            Item currentItem = inventory.DataItem[i];

            // ✅ Bỏ qua nếu không đúng filter
            if (filter.HasValue && currentItem.itemType != filter.Value)
                continue;

            // Nếu là trang bị thì giảm bớt 1 cái (1 cái đang mặc)
            int countToShow = currentItem.isEquippable
                ? Mathf.Max(0, currentItem.amount - 1)
                : currentItem.amount;

            for (int j = 0; j < countToShow; j++)
            {
                if (slotIndex >= slots.Length) return; // hết chỗ

                slots[slotIndex].AddItem(currentItem);

                Item tempItem = currentItem; // tránh capture bug
                slots[slotIndex].button.onClick.AddListener(() =>
                    infoItemUI.UpdateItemInfo(tempItem.BaseStatsItem));

                slotIndex++;
            }
        }
    }

    // 🔘 Gọi khi nhấn nút
    public void ShowConsumables()
    {
        UpdateUI(ItemType.Consumable);
    }

    public void ShowEquipments()
    {
        UpdateUI(ItemType.EquipItem);
    }

    public void ShowAll()
    {
        UpdateUI(null);
    }
}
