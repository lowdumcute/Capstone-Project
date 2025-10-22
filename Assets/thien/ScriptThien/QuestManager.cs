using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("UI References")]
    public TMP_Text questNameText;
    public TMP_Text questValueText;

    private Quest currentQuest;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void AddQuest(string name, int targetValue)
    {
        currentQuest = new Quest(name, targetValue);
        UpdateUI();
    }

    public void CompleteQuestProgress(int amount = 1)
    {
        if (currentQuest == null) return;

        currentQuest.AddProgress(amount);
        UpdateUI();

        if (currentQuest.isCompleted)
        {
            // Hiện chữ Hoàn thành trong name
            questNameText.text = "Hoàn thành";
            questValueText.text = currentQuest.currentValue + "/" + currentQuest.targetValue;

            // Xóa nhiệm vụ sau 2 giây
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
        questValueText.text = currentQuest.currentValue + "/" + currentQuest.targetValue;
    }

    void ClearQuest()
    {
        currentQuest = null;
        UpdateUI();
    }
}
