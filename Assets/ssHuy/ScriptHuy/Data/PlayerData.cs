using UnityEngine;
using System.Collections.Generic;

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
    //  lưu inventory
    public List<ItemData> savedItems = new List<ItemData>();
    // lưu danh sách tên trang bị đã trang bị
    public List<string> equippedItemNames = new List<string>();
    public float healthItem;
    public float manaItem;
    public int attackItem;
    public int armorItem;
    // 🧩 Thêm hai dòng dưới để lưu nhiệm vụ
    public List<QuestData> allQuests = new List<QuestData>();
    public QuestData currentQuest;
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

[System.Serializable]
public class QuestData
{
    public string questName;
    public string questStatus;
    public int currentValue; // Dùng cho QuestDefeatEnemy
    public bool isDefeatEnemyType;
}
