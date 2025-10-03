
using UnityEngine;
[CreateAssetMenu(fileName = "DataGameManager", menuName = "Manager/DataGameManager")]
public class DataGameManager : ScriptableObject
{
    [SerializeField] public BaseStats[] AllRoleStats;
    [SerializeField] public BaseStats playerStatsUsing;
    [SerializeField] public int currentLevel;
    [SerializeField] public int exp;
    [SerializeField] public Vector3 Position;

}
[System.Serializable]
public class GameData
{
    public int level;
    public string Role;
    public Vector3 position;
    public string SceneSave;
}
