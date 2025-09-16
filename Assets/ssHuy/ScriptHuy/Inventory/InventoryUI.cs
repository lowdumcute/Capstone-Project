using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Transform itemsParent; // grid chứa các slot

    Inventory inventory;
    InventorySlot[] slots;
    [SerializeField]InfoItemUI infoItemUI;

    void Start()
    {
        // test
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        inventory = Inventory.instance;
        slots = itemsParent.GetComponentsInChildren<InventorySlot>();
    }

    void Update()
    {
        StatsUI.Instance.UpdateStatsUI();
        UpdateUI();
    }

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.items.Count)
            {
                slots[i].AddItem(inventory.items[i]);

                // Xóa listener cũ trước khi gán mới
                slots[i].button.onClick.RemoveAllListeners();

                // Dùng biến tạm để tránh lỗi capture i
                Item currentItem = inventory.items[i];
                slots[i].button.onClick.AddListener(() => infoItemUI.UpdateItemInfo(currentItem.BaseStatsItem));
            }
            else
            {
                slots[i].ClearSlot();
                slots[i].button.onClick.RemoveAllListeners();
            }
        }
    }

}
