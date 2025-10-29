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
    public float messageDuration = 3f;

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

        // ✅ Chỉ đặt lại sau khi toàn bộ logic chạy xong
        isInteracting = false;
    }

    IEnumerator PlayCompletedDialogue()
    {
        dialoguePanel.SetActive(true);

        //// Khóa input
        //if (InputBlockManager.Instance != null)
        //    InputBlockManager.Instance.BlockInput();


        // ✅ Hiển thị hội thoại hoàn thành
        if (questData.completionMessages != null && questData.completionMessages.Count > 0)
        {
            foreach (string msg in questData.completionMessages)
            {
                yield return StartCoroutine(ShowMessage(msg));

                //  Chờ người chơi nhấn phím để tiếp tục
                yield return new WaitUntil(() =>
                    Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt)
                );

                // Xóa key input tránh nhấn giữ bị skip luôn
                yield return null;
            }
        }
        else
        {
            dialogueText.text = "Cảm ơn con đã đến gặp ta!";
            yield return new WaitUntil(() =>
                Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E)
            );
        }

        // Ẩn bảng hội thoại
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
            Debug.Log("⚠️ Không có phần thưởng để nhận.");
            return;
        }

        foreach (Item item in questData.rewardItems)
        {
            bool added = Inventory.instance.Add(item);

            if (added)
                Debug.Log($" Nhận được {item.BaseStatsItem.name} x{item.Total}");
            else
                Debug.Log($" Không thể thêm {item.BaseStatsItem.name} - Inventory đầy!");
        }
        if (questData.nextQuest != null)
        {
            DialogueManager.Instance.questData = questData.nextQuest;
            questData = DialogueManager.Instance.questData;
            QuestManager.Instance.currentQuest = null;
            QuestManager.Instance.UpdateUI();
        }
    }
}
