using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    [TextArea] public string description;

    public int attack;
    public int speed;
    public int specialAttack;

    public ItemType itemType; // Weapon, Armor, Ring, Consumable...
}

public enum ItemType
{
    Weapon, Armor, Ring, Consumable, Material
}
