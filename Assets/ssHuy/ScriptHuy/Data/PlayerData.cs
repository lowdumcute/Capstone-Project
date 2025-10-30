using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public string role;
    public int level;
    public int exp;
    public Vector3 position;
    public string currentScene;
    public string saveName; // Tên file lưu

    public PlayerData(string role, int level, int exp, Vector3 pos, string scene, string saveName)
    {
        this.role = role;
        this.level = level;
        this.exp = exp;
        position = pos;
        currentScene = scene;
        this.saveName = saveName;
    }
}
