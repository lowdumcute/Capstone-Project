using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardPanel : MonoBehaviour
{
    public static RewardPanel Instance;

    [Header("UI References")]
    public Transform rewardParent; // Grid cha chứa các phần thưởng
    public GameObject rewardItemPrefab; // Prefab cho mỗi item UI (prefab có Image component chính)

    private List<GameObject> rewardItems = new List<GameObject>();

    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Cập nhật danh sách phần thưởng dựa vào current quest
    /// </summary>
    public void UpdateItemInfo()
    {
        // Xóa phần thưởng cũ
        foreach (GameObject go in rewardItems)
            Destroy(go);
        rewardItems.Clear();

        // Kiểm tra dữ liệu quest
        if (QuestManager.Instance == null || QuestManager.Instance.currentQuest == null)
        {
            Debug.LogWarning("⚠️ Không có nhiệm vụ hiện tại để hiển thị phần thưởng.");
            return;
        }

        List<Item> rewards = QuestManager.Instance.currentQuest.rewardItems;
        if (rewards == null || rewards.Count == 0)
        {
            Debug.Log("❌ Nhiệm vụ không có phần thưởng.");
            return;
        }

        // Tạo UI cho từng item
        foreach (Item reward in rewards)
        {
            GameObject itemGO = Instantiate(rewardItemPrefab, rewardParent);
            rewardItems.Add(itemGO);

            // ✅ Lấy Image trực tiếp trên prefab
            Image icon = itemGO.GetComponent<Image>();
            TMP_Text amountText = itemGO.GetComponentInChildren<TMP_Text>(); // vẫn có text con để hiển thị số lượng

            if (icon != null)
                icon.sprite = reward.BaseStatsItem.icon; // Sprite từ ScriptableObject của item
            else
                Debug.LogWarning("⚠️ Prefab không có Image component!");

            if (amountText != null)
                amountText.text =  $"x{reward.Total}";
            else
                Debug.LogWarning("⚠️ Prefab không có TMP_Text component con để hiển thị số lượng!");
        }
    }
}
