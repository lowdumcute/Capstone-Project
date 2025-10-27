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
    private Animator animator;
    private void Start()
    {
        Instance = this;
        animator = GetComponent<Animator>();
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
        if (animator != null)
        {
            animator.SetTrigger("Open");
           Debug.Log("Triggered Open animation for item info UI."); 
        }
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
            UseText.text = "Trang bị";
            if (weapon.Attack > 0)
                StatsText[index++].text = "Tấn Công: " + weapon.Attack;

            if (weapon.Armor > 0)
                StatsText[index++].text = "Phòng thủ: " + weapon.Armor;

            if (weapon.Health > 0)
                StatsText[index++].text = "Máu: " + weapon.Health;

            if (weapon.Mana > 0)
                StatsText[index++].text = "Mana: " + weapon.Mana;

        }
        else if (item is ConsumableItem consumable)
        {
            UseText.text = "Sử dụng";
            if (consumable.restoreHealth > 0)
                StatsText[index++].text = "Khôi phục HP: " + consumable.restoreHealth;

            if (consumable.restoreMana > 0)
                StatsText[index++].text = "Khôi phục MP: " + consumable.restoreMana;
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
