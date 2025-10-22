using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    private Item item;
    private Button button;

    private void Awake()
    {
        // Gán nút nếu có
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnClickSlot);
        }
    }

    public void AddItem(Item newItem)
    {
        item = newItem;
        icon.sprite = item.BaseStatsItem.icon;
        icon.enabled = true;
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    // Khi nhấn vào slot item
    private void OnClickSlot()
    {
        if (item != null && InfoItemUI.Instance != null)
        {

            InfoItemUI.Instance.UpdateItemInfo(item.BaseStatsItem);
        }
    }

    // Nút Remove riêng (nếu cần)
    public void OnRemoveButton()
    {
        if (item != null)
        {
            Inventory.instance.Remove(item.BaseStatsItem, 1);
            InventoryUI.instance.UpdateUI();
        }
    }
}
