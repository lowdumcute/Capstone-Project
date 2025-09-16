using UnityEngine;

public class BaseItem : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public Sprite icon;
    [TextArea] public string description;
}