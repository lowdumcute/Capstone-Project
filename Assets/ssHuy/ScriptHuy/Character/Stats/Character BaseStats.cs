using UnityEngine;
[CreateAssetMenu(menuName = "Base Stats")]
public class BaseStats : ScriptableObject
{
    public string characterName; // chưa dùng
    [Header("Default Stats")]
    public int BAttack; // Sức mạnh
    public int BDefense; // Giáp
    public int BSpeed; // Tốc độ di chuyển
    public int BMana; // Năng lượng
    public int BHealth; // Máu
    
    
    


}
