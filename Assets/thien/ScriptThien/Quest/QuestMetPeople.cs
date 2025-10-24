using UnityEngine;
[CreateAssetMenu(fileName = "New Defeat Enemy Quest", menuName = "Quest/Meet People Quest")]
public class QuestMetPeople : BaseQuest
{
    public string personName;
    public void OnPersonMet(string name)
    {
        // Chỉ tăng tiến độ nếu nhiệm vụ đang được nhận
        if (questStatus != QuestStatus.InProgress) return;
        if (name == personName)
        {
            questStatus = QuestStatus.Completed;
            Debug.Log($"{questName} hoàn thành!");

        }
    }
}
