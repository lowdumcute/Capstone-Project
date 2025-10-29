using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    [Header("UI References")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public Button nextButton;

    [Header("Quest UI")]
    public GameObject questPanel;
    public TMP_Text questNameText;
    public Button acceptQuestButton;

    [Header("Dialogue Settings")]
    public float typingSpeed = 0.05f;

    [Header("Quest Data")]
    public BaseQuest questData; 

    private int currentMessageIndex = 0;
    private Coroutine typingCoroutine;
    private bool showingCompletionDialogue = false;

    [Header("Player Settings")]
    public GameObject playerControllerScript; // GameObject chứa controller
    private ICharacterController playerController;
    public Animator playerAnimator;
    public string npcTag = "TruongLang";
    public float moveSpeedToNpc = 5f;
    public float stopDistance = 1.5f;
    void Update()
    {
        // Khi đang hiển thị panel hội thoại và có thể Next
        if (dialoguePanel.activeSelf && nextButton.gameObject.activeSelf)
        {
            // Nếu người chơi nhấn phím Alt (trái hoặc phải)
            if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
            {
                OnNextClicked();
            }
        }
    }

    void Start()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        // ✅ Tự động tìm player controller (ưu tiên Player_Controller, fallback HS_WhiteMageController)
        if (playerControllerScript != null)
        {
            playerController = playerControllerScript.GetComponent<Player_Controller>() as ICharacterController;
            if (playerController == null)
                playerController = playerControllerScript.GetComponent<HS_WhiteMageController>() as ICharacterController;
        }

        if (playerController == null)
        {
            playerController = FindFirstObjectByType<Player_Controller>() as ICharacterController;
            if (playerController == null)
                playerController = FindFirstObjectByType<HS_WhiteMageController>() as ICharacterController;
        }

        dialoguePanel.SetActive(false);
        nextButton.gameObject.SetActive(false);
        nextButton.onClick.AddListener(OnNextClicked);

        StartCoroutine(StartDialogueAfterDelay(3f));
    }

    public IEnumerator StartDialogueAfterDelay(float timeDelay)
    {
        yield return new WaitForSeconds(timeDelay);
        dialoguePanel.SetActive(true);

        if (InputBlockManager.Instance != null)
            InputBlockManager.Instance.BlockInput();

        // ⚡ Nếu nhiệm vụ đã hoàn thành → hiển thị completionMessages
        if (questData != null && questData.questStatus == QuestStatus.Completed && questData.completionMessages.Count > 0)
        {
            showingCompletionDialogue = true;
            currentMessageIndex = 0;
            ShowMessage(questData.completionMessages[currentMessageIndex]);
        }
        else if (questData != null && questData.messages.Count > 0)
        {
            showingCompletionDialogue = false;
            currentMessageIndex = 0;
            ShowMessage(questData.messages[currentMessageIndex]);
        }
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

        if (questData == null) return;

        if (showingCompletionDialogue)
        {
            // 🔁 Nếu đang hiển thị completionMessages
            if (currentMessageIndex < questData.completionMessages.Count)
            {
                ShowMessage(questData.completionMessages[currentMessageIndex]);
            }
            else
            {
                // ✅ Hết đoạn hội thoại hoàn thành
                dialoguePanel.SetActive(false);
                if (InputBlockManager.Instance != null)
                    InputBlockManager.Instance.UnblockInput();
            }
        }
        else
        {
            // 🔁 Đang hiển thị đoạn hội thoại khi nhận quest
            if (currentMessageIndex < questData.messages.Count)
            {
                ShowMessage(questData.messages[currentMessageIndex]);
            }
            else
            {
                dialoguePanel.SetActive(false);
                StartCoroutine(ShowQuestPanelWithDelay(1f));
            }
        }
    }

    IEnumerator ShowQuestPanelWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowQuestPanel();
    }

    void ShowQuestPanel()
    {
        if (questData == null || questData.questStatus == QuestStatus.Completed) return;

        questPanel.SetActive(true);
        questNameText.text = questData.questName;

        acceptQuestButton.onClick.RemoveAllListeners();
        acceptQuestButton.onClick.AddListener(() =>
        {
            questPanel.SetActive(false);
            questData.questStatus = QuestStatus.InProgress;
            //  Gán nhiệm vụ hiện tại vào QuestManager (tự reset tiến độ)
            QuestManager.Instance.SetQuest(questData);

            // ✅ Nếu có phần thưởng, lưu sẵn trong QuestSO (có thể hiển thị hoặc xử lý sau)
            if (questData.rewardItems != null && questData.rewardItems.Count > 0)
            {
                Debug.Log($"Quest '{questData.questName}' có {questData.rewardItems.Count} vật phẩm thưởng.");
            }

            if (InputBlockManager.Instance != null)
                InputBlockManager.Instance.UnblockInput();
        });
    }
}
