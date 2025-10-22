using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NPCDialogue : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;

    [Header("Reward Panel")]
    public GameObject rewardPanel;
    public Button acceptRewardButton;

    [Header("Dialogue Settings")]
    [TextArea]
    public List<string> messages;
    public float typingSpeed = 0.05f;
    public float messageDuration = 3f;

    [Header("Player Settings")]
    public Player_Controller playerController;
    public float triggerDistance = 2.5f;
    public float delayBeforeReward = 2f; // ⏳ thời gian chờ trước khi hiện panel

    private bool dialogueStarted = false;
    private bool rewardShown = false;
    private Coroutine typingCoroutine;

    void Start()
    {
        dialoguePanel.SetActive(false);
        rewardPanel.SetActive(false);
    }

    void Update()
    {
        if (!dialogueStarted && !rewardShown && playerController != null)
        {
            float distance = Vector3.Distance(playerController.transform.position, transform.position);
            if (distance <= triggerDistance)
            {
                // 🕒 Gọi coroutine để chờ 2 giây rồi mới mở panel
                StartCoroutine(ShowRewardPanelAfterDelay());
            }
        }
    }

    IEnumerator ShowRewardPanelAfterDelay()
    {
        rewardShown = true; // đánh dấu đã gọi để không chạy nhiều lần
        yield return new WaitForSeconds(delayBeforeReward);
        ShowRewardPanel();
    }

    void ShowRewardPanel()
    {
        // 👉 Đánh dấu hoàn thành nhiệm vụ
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.CompleteQuestProgress(1);
        }

        rewardPanel.SetActive(true);

        // Khóa điều khiển người chơi
        if (playerController != null)
            playerController.SetMovementEnabled(false);
        InputBlockManager.Instance.BlockInput();

        acceptRewardButton.onClick.RemoveAllListeners();
        acceptRewardButton.onClick.AddListener(() =>
        {
            // 👉 Thêm phần thưởng vào túi đồ ở đây nếu có
            rewardPanel.SetActive(false);
            StartDialogue();
        });
    }

    void StartDialogue()
    {
        dialogueStarted = true;
        dialoguePanel.SetActive(true);
        StartCoroutine(PlayDialogueSequence());
    }

    IEnumerator PlayDialogueSequence()
    {
        for (int i = 0; i < messages.Count; i++)
        {
            yield return StartCoroutine(ShowMessage(messages[i]));
            yield return new WaitForSeconds(messageDuration);
        }

        // ✅ Kết thúc hội thoại
        dialoguePanel.SetActive(false);
        InputBlockManager.Instance.UnblockInput();
        if (playerController != null)
            playerController.SetMovementEnabled(true);

        // QuestManager.Instance.AddQuest("Đi gặp người canh gác rừng", 1);
    }

    IEnumerator ShowMessage(string message)
    {
        dialogueText.text = "";

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(message));
        yield return typingCoroutine;
    }

    IEnumerator TypeText(string message)
    {
        foreach (char c in message.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
