using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialoguePanel;   // Panel chứa text + button
    public TMP_Text dialogueText;      // Text hiển thị câu
    public Button nextButton;          // Nút Next

    [Header("Dialogue Settings")]
    [TextArea]
    public List<string> messages;      // Danh sách các câu chào
    public float typingSpeed = 0.05f;  // Thời gian in từng ký tự
    public float delayBeforeStart = 3f; // Delay trước khi panel hiện

    private int currentMessageIndex = 0;
    private Coroutine typingCoroutine;

    void Start()
    {
        dialoguePanel.SetActive(false);
        nextButton.gameObject.SetActive(false);
        nextButton.onClick.AddListener(OnNextClicked);

        // Bắt đầu sau 3 giây
        StartCoroutine(StartDialogueAfterDelay());
    }

    IEnumerator StartDialogueAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeStart);
        dialoguePanel.SetActive(true);

        // Khi mở panel thoại -> chặn Attack
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

        // Sau khi in xong hiện nút Next
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
            // Khi kết thúc thoại -> đóng panel
            dialoguePanel.SetActive(false);

            // Cho phép Attack trở lại
            InputBlockManager.Instance.UnblockInput();
        }
    }
}
