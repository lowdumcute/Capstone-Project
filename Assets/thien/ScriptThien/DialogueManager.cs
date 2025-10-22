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
    [Header("Quest UI")]
    public GameObject questPanel;
    public TMP_Text questNameText;
    public Button acceptQuestButton;


    [Header("Player Settings")]
    public GameObject playerControllerScript; // GameObject chứa controller
    private ICharacterController playerController;
    public Animator playerAnimator;
    public string npcTag = "TruongLang";
    public float moveSpeedToNpc = 5f;
    public float stopDistance = 1.5f;

    private GameObject npcTarget;
    private bool movingToNpc = false;

    void Start()
    {
        // ✅ Tự động lấy component hợp lệ từ GameObject
        if (playerControllerScript != null)
        {
            playerController = playerControllerScript.GetComponent<Player_Controller>() as ICharacterController;
            if (playerController == null)
                playerController = playerControllerScript.GetComponent<HS_WhiteMageController>() as ICharacterController;
        }

        // ✅ fallback tìm tự động trong scene
        if (playerController == null)
        {
            playerController = FindFirstObjectByType<Player_Controller>() as ICharacterController;
            if (playerController == null)
                playerController = FindFirstObjectByType<HS_WhiteMageController>() as ICharacterController;
        }
        dialoguePanel.SetActive(false);
        nextButton.gameObject.SetActive(false);
        nextButton.onClick.AddListener(OnNextClicked);

        StartCoroutine(StartDialogueAfterDelay());
    }

    IEnumerator StartDialogueAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeStart);
        dialoguePanel.SetActive(true);

      
        if (InputBlockManager.Instance != null)
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


            // 👉 Hiện panel thông báo nhận nhiệm vụ
            StartCoroutine(ShowQuestPanelWithDelay("Đi tìm trưởng làng", 1f));
        }
    }
    IEnumerator ShowQuestPanelWithDelay(string questName, float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowQuestPanel(questName);
    }
    void ShowQuestPanel(string questName)
    {
        questPanel.SetActive(true);
        questNameText.text = questName;

        // Xóa listener cũ (tránh add nhiều lần)
        acceptQuestButton.onClick.RemoveAllListeners();

        // Khi bấm nút Đồng ý
        acceptQuestButton.onClick.AddListener(() =>
        {
            questPanel.SetActive(false);

            // 👉 Giữ nguyên logic cũ
            QuestManager.Instance.AddQuest(questName, 1);

        
           

            
        });
    }



   
}


