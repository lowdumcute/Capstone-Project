using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class BaseItem : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    [TextArea] public string description;

    public int attack;
    public int speed;
    public int armor;
    public int health;
    public int mana;

    public ItemType itemType; // Weapon, Armor, Ring, Consumable...
}

public enum ItemType
{
    Weapon, Armor, Ring, Consumable, Material
}
