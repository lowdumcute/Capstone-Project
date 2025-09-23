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
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
    }

    // ✅ Thêm item
    public bool Add(Item item)
    {
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

        DataItem.Add(item);
        return true;
    }

    // ✅ Xóa bớt (giảm số lượng)
    public void Remove(BaseItem baseItem, int amount = 1)
    {
        var itemToRemove = DataItem.Find(i => i.BaseStatsItem == baseItem);
        if (itemToRemove != null)
        {
            itemToRemove.amount -= amount;
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
    public int amount = 1; // mặc định 1
    public BaseItem BaseStatsItem;
}

public enum ItemType
{
    EquipItem,
    Consumable,
}
