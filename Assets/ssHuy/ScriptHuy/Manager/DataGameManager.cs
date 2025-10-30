
using UnityEngine;
[CreateAssetMenu(fileName = "DataGameManager", menuName = "Manager/DataGameManager")]
public class DataGameManager : ScriptableObject
{
    [SerializeField] public BaseStats[] AllRoleStats;
    
    [SerializeField] public BaseStats playerStatsUsing;
    [SerializeField] public int currentLevel;
    [SerializeField] public int exp;

    public void changePlayerStats(string playerStatsName)
    {
        BaseStats playerStats = System.Array.Find(AllRoleStats, stats => stats.characterName == playerStatsName);
        if (playerStats == null)
        {
            Debug.LogError($"⚠️ Không tìm thấy BaseStats với tên: {playerStatsName}");
            return;
        }
        playerStatsUsing = playerStats;
    }
}
