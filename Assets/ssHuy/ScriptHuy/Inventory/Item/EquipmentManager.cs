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
    [Header("UI")]
    public StatsUI statsUI;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Equip(BaseItem item)
    {
        foreach (var slot in equipmentSlots)
        {
            // Case 1: item là trang bị
            if (item is EquipItem equip)
            {
                if (slot.slotType == equip.equipType)
                {
                    // Nếu đã có trang bị cũ thì gỡ chỉ số của nó
                    if (slot.currentItem is EquipItem oldEquip)
                    {
                        removeStatsFromEquipment(
                            oldEquip.Attack,
                            oldEquip.Armor,
                            oldEquip.Health,
                            oldEquip.Mana
                        );

                        // Reset trạng thái trong inventory
                        var oldItem = Inventory.instance.FindItem(slot.currentItem);
                        if (oldItem != null)
                            oldItem.isEquippable = false;
                    }

                    // Trang bị item mới
                    EquipToSlot(slot, equip);

                    // Tìm item trong inventory và set trạng thái
                    var newItem = Inventory.instance.FindItem(item);
                    if (newItem != null)
                        newItem.isEquippable = true;

                    // Cộng chỉ số của item mới
                    addStatsFromEquipment(equip.Attack, equip.Armor, equip.Health, equip.Mana);
                    if (InventoryUI.instance != null)
                    {
                        InventoryUI.instance.UpdateUI();
                    }
                    return;
                }
            }
            // Case 2: consumable

            else if (item is ConsumableItem consumable)
            {
                PlayerStats.Instance.Heal(consumable.restoreHealth);
                PlayerStats.Instance.RestoreMana(consumable.restoreMana);//
                statsUI.UpdateStatsUI();
                Inventory.instance.Remove(item, 1);
                InventoryUI.instance.UpdateUI();
                return;
            }
        }

        Debug.LogWarning("No suitable slot found for item: " + item.itemName);
    }
    // Không còn add lại inventory nữa
    private void EquipToSlot(EquipmentSlot slot, BaseItem item)
    {
        slot.currentItem = item;
        slot.CheckItemEquip();
        Debug.Log("Equipped: " + item.itemName);
    }
    public void addStatsFromEquipment(int attack, int armor, float health, float mana)
    {
        AttackItem += attack;
        ArmorItem += armor;
        HealthItem += health;
        ManaItem += mana;
        PlayerStats.Instance.currentHealth += health;
        PlayerStats.Instance.currentMana += mana;
        PlayerStats.Instance.CheckedStats();
        statsUI.UpdateStatsUI();
        UIStatsManager.Instance.UpdateHealth(PlayerStats.Instance.currentHealth, PlayerStats.Instance.maxHealth);
        UIStatsManager.Instance.UpdateMana(PlayerStats.Instance.currentMana, PlayerStats.Instance.maxMana);
    }

    public void removeStatsFromEquipment(int attack, int armor, float health, float mana)
    {
        AttackItem -= attack;
        ArmorItem -= armor;
        HealthItem -= health;
        ManaItem -= mana;
        PlayerStats.Instance.currentHealth -= health;
        PlayerStats.Instance.currentMana -= mana;
        PlayerStats.Instance.CheckedStats();
        statsUI.UpdateStatsUI();
        UIStatsManager.Instance.UpdateHealth(PlayerStats.Instance.currentHealth, PlayerStats.Instance.maxHealth);
        UIStatsManager.Instance.UpdateMana(PlayerStats.Instance.currentMana, PlayerStats.Instance.maxMana);
    }
}
