using UnityEngine;
[CreateAssetMenu(menuName = "Base Stats")]
public class BaseStats : ScriptableObject
{
    public string characterName; // chưa dùng
    public float Health; // Máu
    public float Mana; // Năng lượng
    [Header("Attributes")]
    public float Attack; // Sức mạnh
    public float criticalChance; // Tỷ lệ chí mạng
    public float criticalDamage; // Sát thương chí mạng
    public float attackSpeed; // Tốc độ tấn công
    public float movementSpeed; // Tốc độ di chuyển
    public float Armor; // Giáp


}
