using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    public int space = 36; // số ô trong inventory
    public List<Item> DataItem = new List<Item>();

    void Awake()
    {
        instance = this;
    }

    // ✅ Thêm item
    public bool Add(Item item)
    {
        if (item == null || item.BaseStatsItem == null) return false;

        // Nếu đã có item này trong inventory thì cộng dồn
        var existingItem = DataItem.Find(i => i.BaseStatsItem == item.BaseStatsItem);
        if (existingItem != null)
        {
            existingItem.amount += item.amount;
            return true;
        }

        // Nếu chưa có thì thêm mới
        if (DataItem.Count >= space)
        {
            Debug.Log("Inventory full!");
            return false;
        }

        // Thêm một instance mới (lưu ý: lưu object như hiện tại)
        DataItem.Add(item);
        return true;
    }
    public void Add(BaseItem baseItem, int amount = 1)
    {
        if (baseItem == null) return;

        var existingItem = DataItem.Find(i => i.BaseStatsItem == baseItem);
        if (existingItem != null)
        {
            existingItem.amount += amount;
            return;
        }

        // Nếu chưa có thì thêm mới
        if (DataItem.Count >= space)
        {
            Debug.Log("Inventory full!");
            return;
        }

        // Tạo item mới và thêm vào danh sách
        Item newItem = new Item
        {
            BaseStatsItem = baseItem,
            isEquippable = baseItem is EquipItem
        };
        DataItem.Add(newItem);
    }
    // ✅ Xóa bớt (giảm số lượng)
    public void Remove(BaseItem baseItem, int amount = 1)
    {
        if (baseItem == null) return;

        var itemToRemove = DataItem.Find(i => i.BaseStatsItem == baseItem);
        if (itemToRemove != null)
        {
            itemToRemove.amount -= amount;
            // Không để âm
            if (itemToRemove.amount <= 0)
            {
                DataItem.Remove(itemToRemove);
            }
        }
    }

    public Item FindItem(BaseItem baseItem)
    {
        return DataItem.FirstOrDefault(i => i.BaseStatsItem == baseItem);
    }
}



[System.Serializable]
public class Item
{
    public ItemType itemType;
    public bool isEquippable;
    public int amount = 0;
    public BaseItem BaseStatsItem;
}

public enum ItemType
{
    EquipItem,
    Consumable,
}
