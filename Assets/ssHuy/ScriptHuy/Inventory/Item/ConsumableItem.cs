using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Consumable")]
public class ConsumableItem : BaseItem
{
    public int restoreHealth;
    public int restoreMana;
}

