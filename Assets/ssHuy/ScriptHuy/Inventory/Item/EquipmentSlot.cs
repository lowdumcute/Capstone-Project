using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{
    public EquipType slotType;
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

    public void Unequip()
    {
        if (currentItem != null)
        {
            Item foundItem = Inventory.instance.FindItem(currentItem);

            if (foundItem != null)
            {
                foundItem.isEquippable = false;

                if (currentItem is EquipItem equip)
                {
                    EquipmentManager.instance.removeStatsFromEquipment(
                        equip.Attack,
                        equip.Armor,
                        equip.Health,
                        equip.Mana
                    );
                }
            }

            currentItem = null;
            CheckItemEquip();
            InventoryUI.instance.UpdateUI();
        }
    }
    public void UpdateItemInfo()
    {
        if (currentItem == null) return;
        InfoItemUI.Instance.UpdateItemInfo(currentItem);
    }
}
