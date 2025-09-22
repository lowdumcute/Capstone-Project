using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public Button nextButton;

    [Header("Dialogue Settings")]
    [TextArea]
    public List<string> messages;
    public float typingSpeed = 0.05f;
    public float delayBeforeStart = 3f;

    private int currentMessageIndex = 0;
    private Coroutine typingCoroutine;

    [Header("Player Settings")]
    public Player_Controller playerController; // Player_Controller script
    public Animator playerAnimator;
    public string npcTag = "TruongLang";
    public float moveSpeedToNpc = 5f;
    public float stopDistance = 1.5f;

    private GameObject npcTarget;
    private bool movingToNpc = false;

    void Start()
    {
        dialoguePanel.SetActive(false);
        nextButton.gameObject.SetActive(false);
        nextButton.onClick.AddListener(OnNextClicked);

        StartCoroutine(StartDialogueAfterDelay());
    }

    IEnumerator StartDialogueAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeStart);
        dialoguePanel.SetActive(true);

        // Khóa điều khiển tay người chơi
        playerController.SetMovementEnabled(false);
        InputBlockManager.Instance.BlockInput();

        ShowMessage(messages[currentMessageIndex]);
    }

    void ShowMessage(string message)
    {
        dialogueText.text = "";
        nextButton.gameObject.SetActive(false);

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(message));
    }

    IEnumerator TypeText(string message)
    {
        foreach (char c in message.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        nextButton.gameObject.SetActive(true);
    }

    void OnNextClicked()
    {
        currentMessageIndex++;

        if (currentMessageIndex < messages.Count)
        {
            ShowMessage(messages[currentMessageIndex]);
        }
        else
        {
            dialoguePanel.SetActive(false);
            InputBlockManager.Instance.UnblockInput();

            // 👉 Khi kết thúc hội thoại -> thêm nhiệm vụ vào danh sách
            QuestManager.Instance.AddQuest("Đi tìm trưởng làng", 1);

            // 👉 Sau đó player tự động di chuyển đến NPC
            MovePlayerToNPC();
        }
    }

    void MovePlayerToNPC()
    {
        npcTarget = GameObject.FindGameObjectWithTag(npcTag);
        if (npcTarget != null)
        {
            movingToNpc = true;
            StartCoroutine(MoveToNpcRoutine());
        }
        else
        {
            // Dừng lại
            playerAnimator.SetBool("isRun", false);
            movingToNpc = false;

            // 👉 Đánh dấu hoàn thành nhiệm vụ
            QuestManager.Instance.CompleteQuestProgress(1);

            // Cho phép player điều khiển lại
            playerController.SetMovementEnabled(true);
        }
    }

    IEnumerator MoveToNpcRoutine()
    {
        while (movingToNpc && npcTarget != null)
        {
            Vector3 direction = (npcTarget.transform.position - playerController.transform.position);
            direction.y = 0;

            float distance = direction.magnitude;

            if (distance > stopDistance)
            {
                // Hướng tới NPC
                direction.Normalize();

                // Quay mặt
                Quaternion targetRot = Quaternion.LookRotation(direction);
                playerController.transform.rotation = Quaternion.Slerp(
                    playerController.transform.rotation,
                    targetRot,
                    Time.deltaTime * 10f
                );

                // Di chuyển tới NPC
                playerController.transform.position += direction * moveSpeedToNpc * Time.deltaTime;

                // 👉 Bật animation chạy
                playerAnimator.SetBool("isRun", true);
            }
            else
            {
                // Dừng lại
                playerAnimator.SetBool("isRun", false);
                movingToNpc = false;

                // 👉 Đánh dấu hoàn thành nhiệm vụ
                if (QuestManager.Instance != null)
                {
                    QuestManager.Instance.CompleteQuestProgress(1);
                }

                // Cho phép player điều khiển lại
                playerController.SetMovementEnabled(true);
            }

            yield return null;
        }
    }

}
