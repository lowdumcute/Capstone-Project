using UnityEngine;

[System.Serializable]
public class Quest
{
    public string questName;
    public int currentValue;
    public int targetValue;
    public bool isCompleted;

    public Quest(string name, int target)
    {
        questName = name;
        targetValue = target;
        currentValue = 0;
        isCompleted = false;
    }

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
}
