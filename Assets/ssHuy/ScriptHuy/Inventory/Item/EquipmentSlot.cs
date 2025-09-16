using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{
    public ItemType slotType;
    public BaseItem currentItem;
    [SerializeField] Image icon;

    private void Start()
    {
        CheckItemEquip();
    }

    public void CheckItemEquip()
    {
        if (currentItem != null)
        {
            icon.sprite = currentItem.icon;
            icon.enabled = true;
        }
        else
        {
            icon.enabled = false;
        }
    }

    // Nếu bạn muốn gỡ trang bị thủ công
    public void Unequip()
    {
        if (currentItem != null)
        {
            Inventory.instance.Add(new Item 
            { 
                isEquippable = true, 
                amount = 1, 
                BaseStatsItem = currentItem 
            });

            currentItem = null;
            CheckItemEquip();
        }
    }
}
