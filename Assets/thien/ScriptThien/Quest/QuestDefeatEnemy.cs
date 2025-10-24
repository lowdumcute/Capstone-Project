using UnityEngine;

[CreateAssetMenu(fileName = "New Defeat Enemy Quest", menuName = "Quest/Defeat Enemy Quest")]
public class QuestDefeatEnemy : BaseQuest
{
    [Header("Enemy Target Settings")]
    public string targetEnemyName;
    public int currentValue = 0;
    public int targetValue = 1;

    // ✅ Gọi hàm này mỗi khi 1 enemy chết
    public void OnEnemyDefeated(string enemy)
    {
        // Chỉ tăng tiến độ nếu nhiệm vụ đang được nhận
        if (!isTaken) return;
        if (enemy == targetEnemyName)
        {
            AddProgress(1);
            Debug.Log($"{questName}: Đã tiêu diệt {currentValue}/{targetValue}");

            if (isCompleted)
            {
                Debug.Log($"{questName} hoàn thành!");
            }
        }
    }
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
