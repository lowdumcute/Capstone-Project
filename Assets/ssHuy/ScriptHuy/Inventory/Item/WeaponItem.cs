using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Weapon")]
public class EquipItem : BaseItem
{
    public EquipType equipType;
    public int Attack;
    public int Armor;
    public int Health;
    public int Mana;
}
public enum EquipType
{
    Hat,
    Gloves,
    Shoes,
    Armor,
    Ring
}
