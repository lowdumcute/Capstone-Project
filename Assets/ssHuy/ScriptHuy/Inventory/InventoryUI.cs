using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance;
    public Transform itemsParent; // grid chứa các slot

    Inventory inventory;
    InventorySlot[] slots;
    [SerializeField] InfoItemUI infoItemUI;

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

        UpdateUI(); // Cập nhật lần đầu
    }

    void OnEnable()
    {
        // Nếu inventory đã có dữ liệu thì tự động cập nhật khi mở UI
        if (Inventory.instance != null)
        {
            inventory = Inventory.instance;
            if (slots == null || slots.Length == 0)
                slots = itemsParent.GetComponentsInChildren<InventorySlot>();

            UpdateUI();
        }
    }

    void Update()
    {
        // Cập nhật thông tin stat nhân vật (nếu cần)
        StatsUI.Instance.UpdateStatsUI();
    }

    /// <summary>
    /// Cập nhật giao diện inventory, có thể lọc theo loại item
    /// </summary>
    public void UpdateUI(ItemType? filter = null)
    {
        if (inventory == null)
            inventory = Inventory.instance;

        if (inventory == null) return;

        // Xóa toàn bộ slot trước
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].ClearSlot();
            slots[i].button.onClick.RemoveAllListeners();
        }

        int slotIndex = 0;

        for (int i = 0; i < inventory.DataItem.Count; i++)
        {
            Item currentItem = inventory.DataItem[i];

            // Lọc nếu cần
            if (filter.HasValue && currentItem.itemType != filter.Value)
                continue;

            // Nếu là item trang bị thì trừ đi 1 cái đang mặc
            int countToShow = currentItem.isEquippable
                ? Mathf.Max(0, currentItem.amount - 1)
                : currentItem.amount;

            for (int j = 0; j < countToShow; j++)
            {
                if (slotIndex >= slots.Length)
                    return; // hết slot

                slots[slotIndex].AddItem(currentItem);

                Item tempItem = currentItem; // tránh capture bug
                slots[slotIndex].button.onClick.AddListener(() =>
                    infoItemUI.UpdateItemInfo(tempItem.BaseStatsItem));

                slotIndex++;
            }
        }
    }

    // 🔘 Gọi khi nhấn nút lọc
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
