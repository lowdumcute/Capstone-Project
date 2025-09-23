using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InfoItemUI : MonoBehaviour
{
    public static InfoItemUI Instance { get; private set; }

    [SerializeField] private Image itemIcon;
    private BaseItem currenIitem;

    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI[] StatsText;
    public TextMeshProUGUI UseText;

    private void Start()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        itemIcon.enabled = false;
        itemNameText.text = "";
        descriptionText.text = "";
        UseText.text = "";
        ClearStats();
    }

    public void UpdateItemInfo(BaseItem item)
    {
        currenIitem = item;

        itemIcon.enabled = true;
        itemIcon.sprite = item.icon;

        itemNameText.text = item.itemName;
        descriptionText.text = item.description;

        UpdateStatsUI(item);
    }

    private void ClearStats()
    {
        for (int i = 0; i < StatsText.Length; i++)
        {
            StatsText[i].text = "";
        }
    }

    // update chỉ số item theo loại
    private void UpdateStatsUI(BaseItem item)
    {
        ClearStats();
        int index = 0; // để ghi lần lượt vào StatsText

        if (item is EquipItem weapon)
        {
            UseText.text = "Equip";
            if (weapon.Attack > 0)
                StatsText[index++].text = "Attack: " + weapon.Attack;

            if (weapon.Armor > 0)
                StatsText[index++].text = "Armor: " + weapon.Armor;

            if (weapon.Health > 0)
                StatsText[index++].text = "Health: " + weapon.Health;

            if (weapon.Mana > 0)
                StatsText[index++].text = "Mana: " + weapon.Mana;
        }
        else if (item is ConsumableItem consumable)
        {
            UseText.text = "Use";
            if (consumable.restoreHealth > 0)
                StatsText[index++].text = "Restore HP: " + consumable.restoreHealth;

            if (consumable.restoreMana > 0)
                StatsText[index++].text = "Restore MP: " + consumable.restoreMana;
        }
        else
        {
            StatsText[0].text = "Không có chỉ số đặc biệt";
        }
    }

    public void EquipItem()
    {
        if (currenIitem != null)
        {
            EquipmentManager.instance.Equip(currenIitem);
        }
    }

    public void UnEquipItem()
    {
        if (currenIitem != null)
        {
            foreach (var slot in EquipmentManager.instance.equipmentSlots)
            {
                if (slot.currentItem == currenIitem)
                {
                    slot.Unequip();
                    return;
                }
            }
        }
    }
}
