using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCDialogue : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;

    [Header("Dialogue Settings")]
    [TextArea]
    public List<string> messages;
    public float typingSpeed = 0.05f;
    public float messageDuration = 3f; // Thời gian mỗi câu thoại trước khi tự động chuyển

    [Header("Player Settings")]
    public Player_Controller playerController; // Tham chiếu script điều khiển Player
    public float triggerDistance = 2.5f; // Khoảng cách để bắt đầu thoại

    private bool dialogueStarted = false;
    private Coroutine typingCoroutine;

    void Start()
    {
        dialoguePanel.SetActive(false);
    }

    void Update()
    {
        if (!dialogueStarted && playerController != null)
        {
            float distance = Vector3.Distance(playerController.transform.position, transform.position);
            if (distance <= triggerDistance)
            {
                StartDialogue();
            }
        }
    }

    void StartDialogue()
    {
        dialogueStarted = true;
        dialoguePanel.SetActive(true);

        // Khóa điều khiển player
        if (playerController != null)
            playerController.SetMovementEnabled(false);
        InputBlockManager.Instance.BlockInput();

        StartCoroutine(PlayDialogueSequence());
    }

    IEnumerator PlayDialogueSequence()
    {
        for (int i = 0; i < messages.Count; i++)
        {
            yield return StartCoroutine(ShowMessage(messages[i]));
            yield return new WaitForSeconds(messageDuration);
        }

        // Kết thúc hội thoại
        dialoguePanel.SetActive(false);
        InputBlockManager.Instance.UnblockInput();
        if (playerController != null)
            playerController.SetMovementEnabled(true);

        // 👉 Thêm nhiệm vụ vào QuestManager
        //if (QuestManager.Instance != null)
        //{
        //    QuestManager.Instance.AddQuest("Tiêu diệt Quái Vật Xương Trắng", 1);
        //}
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
