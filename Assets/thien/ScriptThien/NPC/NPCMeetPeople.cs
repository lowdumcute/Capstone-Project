using System.Collections;
using UnityEngine;

public class NPCMeetPeople : NPC
{
    private bool isInteracting = false;

    protected override void Update()
    {
        if (playerController == null || questData == null || isInteracting) return;

        float distance = Vector3.Distance(playerController.transform.position, transform.position);

        // Khi người chơi ở gần và nhấn phím E
        if (distance <= triggerDistance && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(HandleInteraction());
        }
    }

    IEnumerator HandleInteraction()
    {
        isInteracting = true;

        // ✅ Nếu chưa hoàn thành thì đánh dấu hoàn thành
        if (!questData.isCompleted)
            questData.isCompleted = true;

        // ✅ Chạy đoạn hội thoại hoàn thành
        yield return StartCoroutine(PlayCompletedDialogue());

        // ✅ Sau hội thoại → hiện bảng nhận thưởng
        ShowRewardPanel();

        isInteracting = false;
    }

    IEnumerator PlayCompletedDialogue()
    {
        dialoguePanel.SetActive(true);

        // Khóa input
        if (InputBlockManager.Instance != null)
            InputBlockManager.Instance.BlockInput();

        if (playerController != null)
            playerController.SetMovementEnabled(false);

        // ✅ Hiển thị hội thoại hoàn thành
        if (questData.completionMessages != null && questData.completionMessages.Count > 0)
        {
            foreach (string msg in questData.completionMessages)
            {
                yield return StartCoroutine(ShowMessage(msg));
                yield return new WaitForSeconds(messageDuration);
            }
        }
        else
        {
            dialogueText.text = "Cảm ơn con đã đến gặp ta!";
            yield return new WaitForSeconds(messageDuration);
        }

        // Ẩn bảng hội thoại
        dialoguePanel.SetActive(false);
    }
}
