using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public string npcName;
    [Header("UI References")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;

    [Header("Reward Panel")]
    public GameObject rewardPanel;
    public Button acceptRewardButton;

    [Header("Quest Data")]
    public BaseQuest questData;

    [Header("Dialogue Settings")]
    public float typingSpeed = 0.05f;
    public float nextLineDelay = 0.5f; // ⏱ Thời gian chờ sau khi hết chữ

    [Header("Player Settings")]
    public Player_Controller playerController;
    public float triggerDistance = 2.5f;
    private bool isInteracting = false;
    protected Coroutine typingCoroutine;

    protected virtual void Start()
    {
        dialoguePanel.SetActive(false);
        rewardPanel.SetActive(false);
    }

    protected virtual void Update()
    {
        if (playerController == null || questData == null || isInteracting) return;

        float distance = Vector3.Distance(playerController.transform.position, transform.position);

        if (distance <= triggerDistance && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(HandleInteraction());
        }
    }

    IEnumerator HandleInteraction()
    {
        isInteracting = true;

        if (questData is QuestDefeatEnemy defeatQuest)
        {
            if (defeatQuest.questStatus == QuestStatus.Available)
            {
                yield return DialogueManager.Instance.StartDialogueAfterDelay(0f);
            }
            else if (defeatQuest.questStatus == QuestStatus.Completed)
            {
                yield return StartCoroutine(PlayCompletedDialogue());
                ShowRewardPanel();
            }
        }
        else if (QuestManager.Instance.currentQuest is QuestMetPeople meetQuest)
        {
            meetQuest.OnPersonMet(npcName);
            if (meetQuest.questStatus == QuestStatus.Completed)
            {
                yield return StartCoroutine(PlayCompletedDialogue());
                ShowRewardPanel();
            }
        }

        isInteracting = false;
    }

    IEnumerator PlayCompletedDialogue()
    {
        dialoguePanel.SetActive(true);

        if (questData.completionMessages != null && questData.completionMessages.Count > 0)
        {
            foreach (string msg in questData.completionMessages)
            {
                // Gọi coroutine hiển thị và đợi hoàn tất typing
                yield return StartCoroutine(ShowMessage(msg));

                // ⏱ Chờ 0.5s trước khi qua dòng kế tiếp
                yield return new WaitForSeconds(nextLineDelay);
            }
        }
        else
        {
            dialogueText.text = "Cảm ơn con đã đến gặp ta!";
            yield return new WaitForSeconds(nextLineDelay);
        }

        dialoguePanel.SetActive(false);
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
        });
    }

    protected virtual void GiveRewards()
    {
        if (questData == null || questData.rewardItems.Count == 0)
        {
            Debug.Log(" Không có phần thưởng để nhận.");
            return;
        }

        foreach (Item item in questData.rewardItems)
        {
            questData.questStatus = QuestStatus.Rewarded;
            bool added = Inventory.instance.Add(item);

            if (added)
                Debug.Log($" Nhận được {item.BaseStatsItem.name} x{item.amount}");
            else
                Debug.Log($" Không thể thêm {item.BaseStatsItem.name} - Inventory đầy!");
        }
        if (questData.nextQuest != null)
        {
            QuestManager.Instance.UpdateUI();
            DialogueManager.Instance.questData = questData.nextQuest;
            questData = DialogueManager.Instance.questData;
            QuestManager.Instance.currentQuest = null;
        }
    }
}
