using System.Collections.Generic;
using UnityEngine;
public enum QuestStatus
{
    Available,
    InProgress,
    Completed
}
[CreateAssetMenu(fileName = "New Quest", menuName = "Quest/New Quest")]
public class BaseQuest : ScriptableObject
{
    [Header("Quest Info")]
    public string questName;
    public string questDescription;
    public QuestStatus questStatus ;

    [Header("Dialogue Lines - Before Quest")]   
    [TextArea]
    public List<string> messages = new List<string>();

    [Header("Dialogue Lines - After Completion")]
    [TextArea]
    public List<string> completionMessages = new List<string>(); // 🏁 thoại sau khi hoàn thành
    [Header("Quest Flow")]
    public BaseQuest nextQuest; // 👉 Nhiệm vụ tiếp theo trong chuỗi

    [Header("Quest Target")]
    public Vector3 questLocation; // Vị trí nơi làm nhiệm vụ

    [Header("Reward Items")]
    public List<Item> rewardItems = new List<Item>();
}
