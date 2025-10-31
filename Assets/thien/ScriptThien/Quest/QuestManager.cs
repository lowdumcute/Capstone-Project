using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("UI References")]
    public TMP_Text questNameText;
    public TMP_Text questValueText;

    [Header("Quest Data")]
    public List<BaseQuest> allQuests; // Danh sách tất cả nhiệm vụ trong game
    public BaseQuest currentQuest; // quest hiện tại của người chơi
    [SerializeField] GameObject QuestMarkerPrefab; // Prefab cột sáng nhiệm vụ

    private GameObject currentMarker; // Giữ marker hiện tại

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        CheckQuestStatus();
    }

    public void SetQuest(BaseQuest quest)
    {
        if (quest == null) return;

        currentQuest = quest;

        CheckQuestStatus();
        UpdateUI();
    }

    public void CompleteQuestProgress()
    {
        if (currentQuest == null) return;
        UpdateUI();

        // Nếu hoàn thành nhiệm vụ
        if (currentQuest.questStatus == QuestStatus.Completed)
        {
            if (currentMarker != null) Destroy(currentMarker);

            questNameText.text = $"Hoàn thành: {currentQuest.questName}";
            if (currentQuest is QuestDefeatEnemy enemyQuest)
            {
                questValueText.text = $"{enemyQuest.currentValue}/{enemyQuest.targetValue}";
            }
            else
            {
                questValueText.text = $"{currentQuest.questDescription}";
            }

            // Xóa UI sau 3 giây
            Invoke(nameof(ClearQuest), 3f);
        }
    }

    public void CheckQuestStatus()
    {
        if (currentQuest == null) return;

        // Xóa marker nếu quest hoàn thành
        if (currentQuest.questStatus == QuestStatus.Completed)
        {
            if (currentMarker != null) Destroy(currentMarker);
            return;
        }

        // Spawn marker nếu nhiệm vụ đang nhận và có location
        if (currentQuest.questStatus == QuestStatus.InProgress && currentQuest.questLocation != Vector3.zero)
        {
            if (currentMarker != null) Destroy(currentMarker);

            currentMarker = Instantiate(
                QuestMarkerPrefab,
                currentQuest.questLocation,
                Quaternion.identity
            );
        }
        else
        {
            if (currentMarker != null) Destroy(currentMarker);
        }
    }

    public void UpdateUI()
    {
        // Nếu không có quest hoặc đã nhận thưởng => clear text
        if (currentQuest == null || currentQuest.questStatus == QuestStatus.Rewarded)
        {
            questNameText.text = "";
            questValueText.text = "";
            return;
        }

        // Cập nhật tên nhiệm vụ
        questNameText.text = currentQuest.questName;

        // Nếu là nhiệm vụ tiêu diệt quái thì hiển thị tiến độ
        if (currentQuest is QuestDefeatEnemy enemyQuest)
        {
            questValueText.text = $"{enemyQuest.questDescription}: {enemyQuest.currentValue}/{enemyQuest.targetValue}";
        }
        else
        {
            questValueText.text = $"{currentQuest.questDescription}";
        }
    }


    void ClearQuest()
    {
        currentQuest = null;
        UpdateUI();
    }

}
