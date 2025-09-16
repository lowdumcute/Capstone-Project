using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class InfoItemUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    private BaseItem currenIitem;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI[] StatsText;


    public void OnEnable()
    {
        itemIcon.enabled = false;
        itemNameText.text = "";
        descriptionText.text = "";
        for (int i = 0; i < StatsText.Length; i++)
        {
            StatsText[i].text = "";
        }
    }
    public void UpdateItemInfo(BaseItem item)
    {
        itemIcon.enabled = true;
        currenIitem = item;
        itemIcon.sprite = item.icon;
        itemNameText.text = item.itemName;
        descriptionText.text = item.description;

        UpdateStatsUI(item);
    }
    // update chỉ số item 
    private void UpdateStatsUI(BaseItem item)
    {
        StatsText[0].text = "Attack: " + item.attack;
        StatsText[1].text = "Armor: " + item.armor;
        StatsText[2].text = "Health: " + item.health;
        StatsText[3].text = "Mana: " + item.mana;
        StatsText[4].text = "Speed: " + item.speed;
    }
    public void EquipItem()
    {
        if (currenIitem != null)
        {
            EquipmentManager.instance.Equip(currenIitem);
        }
    }
}
