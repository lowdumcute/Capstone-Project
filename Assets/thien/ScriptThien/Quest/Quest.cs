using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest/New Quest")]
public class QuestSO : ScriptableObject
{
    [Header("Quest Info")]
    public string questName;
    public string questDescription;
    public int targetValue = 1;

    [Header("Dialogue Lines - Before Quest")]
    [TextArea]
    public List<string> messages = new List<string>();

    [Header("Dialogue Lines - After Completion")]
    [TextArea]
    public List<string> completionMessages = new List<string>(); // 🏁 thoại sau khi hoàn thành

    [Header("Reward Items")]
    public List<Item> rewardItems = new List<Item>();

    [HideInInspector] public int currentValue = 0;
    public bool isCompleted = false;

    // ✅ Thêm tiến độ cho quest
    public void AddProgress(int amount = 1)
    {
        if (isCompleted) return;

        currentValue += amount;
        if (currentValue >= targetValue)
        {
            currentValue = targetValue;
            isCompleted = true;
        }
    }

    // ✅ Reset lại tiến độ (khi gán quest mới)
    public void ResetProgress()
    {
        currentValue = 0;
        isCompleted = false;
    }
}
