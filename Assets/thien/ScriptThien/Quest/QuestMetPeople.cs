using UnityEngine;
[CreateAssetMenu(fileName = "New Defeat Enemy Quest", menuName = "Quest/Meet People Quest")]
public class QuestMetPeople : BaseQuest
{
    public string personName;
    public void OnPersonMet(string name)
    {
        // Chỉ tăng tiến độ nếu nhiệm vụ đang được nhận
        if (!isTaken) return;
        if (name == personName)
        {
            isCompleted = true;
            Debug.Log($"{questName} hoàn thành!");

        }
    }
}
