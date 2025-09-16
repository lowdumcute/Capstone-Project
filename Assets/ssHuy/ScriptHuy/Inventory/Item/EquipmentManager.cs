using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public List<EquipmentSlot> equipmentSlots;
    public static EquipmentManager instance;
    [Header("Stats Items")]
    public float HealthItem;
    public float ManaItem;
    public int AttackItem;
    public int ArmorItem;

    void Start()
    {
        instance = this;
    }

    public void Equip(BaseItem item)
    {
        foreach (var slot in equipmentSlots)
        {
            // Case 1: item để trang bị (vũ khí, giáp, nhẫn...)
            if (item is EquipItem equip)
            {
                if (slot.slotType == equip.equipType)
                {
                    
                    EquipToSlot(slot, equip);
                    addStatsFromEquipment(equip.Attack, equip.Armor, equip.Health, equip.Mana);
                    return;
                }
            }

            // Case 2: consumable (thuốc máu/mana) -> cho vào slot Consumable
            else if (item is ConsumableItem consumable)
            {
                PlayerStats.Instance.Heal(consumable.restoreHealth);
                PlayerStats.Instance.UseMana(consumable.restoreMana);
                StatsUI.Instance.UpdateStatsUI();
                Inventory.instance.Remove(item,1);
                return; // ✅ nhớ return, không để chạy xuống Debug.LogWarning
            }
        }

        Debug.LogWarning("No suitable slot found for item: " + item.itemName);
    }

    // Hàm con để tránh lặp code
    private void EquipToSlot(EquipmentSlot slot, BaseItem item)
    {
        // Nếu đã có item trong slot thì trả lại inventory
        if (slot.currentItem != null)
        {
            Inventory.instance.Add(new Item
            {
                isEquippable = true,
                amount = 1,
                BaseStatsItem = slot.currentItem
            });
        }

        // Trang bị item mới
        slot.currentItem = item;
        slot.CheckItemEquip();   // ✅ cập nhật lại UI icon

        Debug.Log("Equipped: " + item.itemName);
    }
    public void addStatsFromEquipment(int attack, int armor, float health, float mana)
    {
        AttackItem += attack;
        ArmorItem += armor;
        HealthItem += health;
        ManaItem += mana;

        PlayerStats.Instance.currentHealth += health; // tăng máu hiện tại khi thêm máu tối đa
        PlayerStats.Instance.currentMana += mana;     // tăng mana hiện tại khi thêm mana tối đa
        PlayerStats.Instance.CheckedStats(); // tinh lại chỉ số
        StatsUI.Instance.UpdateStatsUI();
        UIStatsManager.Instance.UpdateHealth(PlayerStats.Instance.currentHealth, PlayerStats.Instance.maxHealth);
        UIStatsManager.Instance.UpdateMana(PlayerStats.Instance.currentMana, PlayerStats.Instance.maxMana);
        
    }
}
