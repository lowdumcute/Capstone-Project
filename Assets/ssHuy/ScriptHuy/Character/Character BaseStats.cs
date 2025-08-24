using UnityEngine;
[CreateAssetMenu(menuName = "Base Stats")]
public class CharacterBaseStats : ScriptableObject
{
    public string characterName; // chưa dùng
    public float maxHealth; // Máu
    public float maxMana; // Năng lượng
    [Header("Attributes")]
    public float strength; // Sức mạnh
    public float agility; // Sự nhanh nhẹn
    public float intelligence; // Trí tuệ
    public float stamina; // Sức bền
    public float criticalChance; // Tỷ lệ chí mạng
    public float criticalDamage; // Sát thương chí mạng
    public float attackSpeed; // Tốc độ tấn công
    public float movementSpeed; // Tốc độ di chuyển
    public float armor; // Giáp


}
