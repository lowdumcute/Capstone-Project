using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("UI References")]
    public TMP_Text questNameText;
    public TMP_Text questValueText;

    [Header("Quest Data")]
    public QuestSO currentQuest; // 🧩 Trỏ đến ScriptableObject

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Gán nhiệm vụ hiện tại từ QuestSO (và reset tiến độ nếu cần)
    /// </summary>
    public void SetQuest(QuestSO quest, bool resetProgress = true)
    {
        if (quest == null) return;

        currentQuest = quest;
        if (resetProgress) currentQuest.ResetProgress();

        UpdateUI();
    }

    /// <summary>
    /// Tăng tiến độ nhiệm vụ hiện tại
    /// </summary>
    public void CompleteQuestProgress(int amount = 1)
    {
        if (currentQuest == null) return;

        currentQuest.AddProgress(amount);
        UpdateUI();

        if (currentQuest.isCompleted)
        {
            questNameText.text = "Hoàn thành: " + currentQuest.questName;
            questValueText.text = currentQuest.currentValue + "/" + currentQuest.targetValue;

            // Xóa UI sau 3 giây
            Invoke(nameof(ClearQuest), 3f);
        }
    }

    void UpdateUI()
    {
        if (currentQuest == null)
        {
            questNameText.text = "";
            questValueText.text = "";
            return;
        }

        questNameText.text = currentQuest.questName;
        questValueText.text = $"{currentQuest.currentValue}/{currentQuest.targetValue}";
    }

    void ClearQuest()
    {
        currentQuest = null;
        UpdateUI();
    }
}
