using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;

    [Header("Reward Panel")]
    public GameObject rewardPanel;
    public Button acceptRewardButton;

    [Header("Quest Data")]
    public QuestSO questData;

    [Header("Dialogue Settings")]
    public float typingSpeed = 0.05f;
    public float messageDuration = 3f;

    [Header("Player Settings")]
    public Player_Controller playerController;
    public float triggerDistance = 2.5f;

    protected Coroutine typingCoroutine;

    protected virtual void Start()
    {
        dialoguePanel.SetActive(false);
        rewardPanel.SetActive(false);
    }

    protected virtual void Update()
    {
        // Lớp cha không xử lý hành vi, để lớp con override
    }

    protected IEnumerator ShowMessage(string message)
    {
        dialogueText.text = "";

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(message));
        yield return typingCoroutine;
    }

    protected IEnumerator TypeText(string message)
    {
        foreach (char c in message)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    protected virtual void ShowRewardPanel()
    {
        rewardPanel.SetActive(true);
        RewardPanel.Instance.UpdateItemInfo();
        acceptRewardButton.onClick.RemoveAllListeners();
        acceptRewardButton.onClick.AddListener(() =>
        {
            GiveRewards();
            rewardPanel.SetActive(false);

            InputBlockManager.Instance.UnblockInput();
            if (playerController != null)
                playerController.SetMovementEnabled(true);
        });
    }

    protected virtual void GiveRewards()
    {
        if (questData == null || questData.rewardItems.Count == 0)
        {
            Debug.Log("⚠️ Không có phần thưởng để nhận.");
            return;
        }

        foreach (Item item in questData.rewardItems)
        {
            bool added = Inventory.instance.Add(item);

            if (added)
                Debug.Log($"✅ Nhận được {item.BaseStatsItem.name} x{item.amount}");
            else
                Debug.Log($"❌ Không thể thêm {item.BaseStatsItem.name} - Inventory đầy!");
        }
    }
}
