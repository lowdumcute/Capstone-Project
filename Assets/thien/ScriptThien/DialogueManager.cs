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

    [Header("Dialogue Settings")]
    [TextArea]
    public List<string> messages;
    public float typingSpeed = 0.05f;
    public float delayBeforeStart = 3f;

    private int currentMessageIndex = 0;
    private Coroutine typingCoroutine;
    private bool canPressNext = false;

    [Header("Quest UI")]
    public GameObject questPanel;
    public TMP_Text questNameText;
    public Button acceptQuestButton;

    [Header("Player Settings")]
    public GameObject playerControllerScript;
    private ICharacterController playerController;

    public Animator playerAnimator;
    public string npcTag = "TruongLang";

    [Header("Quest Marker")]
    public GameObject questMarkerPrefab; // ✅ Prefab cột sáng
    private GameObject currentMarkerInstance;
    public float markerHeightOffset = 2f;
    public float removeMarkerDistance = 2f; // ✅ khoảng cách để tự xoá marker

    private GameObject npcTarget;

    void Start()
    {
        // ✅ tự động lấy component hợp lệ từ GameObject
        if (playerControllerScript != null)
        {
            playerController = playerControllerScript.GetComponent<Player_Controller>() as ICharacterController;
            if (playerController == null)
                playerController = playerControllerScript.GetComponent<HS_WhiteMageController>() as ICharacterController;
        }

        // ✅ fallback tìm tự động
        if (playerController == null)
        {
            playerController = FindFirstObjectByType<Player_Controller>() as ICharacterController;
            if (playerController == null)
                playerController = FindFirstObjectByType<HS_WhiteMageController>() as ICharacterController;
        }

        if (playerController == null)
        {
            Debug.LogError("❌ DialogueManager: playerController vẫn null! Hãy kiểm tra Player_Controller hoặc HS_WhiteMageController có trong scene không.");
        }

        dialoguePanel.SetActive(false);
        StartCoroutine(StartDialogueAfterDelay());
    }

    void Update()
    {
        // ✅ cho phép bấm B khi in xong
        if (canPressNext && Input.GetKeyDown(KeyCode.B))
        {
            OnNextPressed();
        }

        // ✅ kiểm tra tự xoá marker khi người chơi lại gần
        if (currentMarkerInstance != null && playerController != null && npcTarget != null)
        {
            float dist = Vector3.Distance(playerController.transform.position, npcTarget.transform.position);
            if (dist <= removeMarkerDistance)
            {
                Destroy(currentMarkerInstance);
                currentMarkerInstance = null;

                // ✅ đánh dấu quest tiến độ xong
                if (QuestManager.Instance != null)
                    QuestManager.Instance.CompleteQuestProgress(1);
            }
        }
    }

    IEnumerator StartDialogueAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeStart);
        dialoguePanel.SetActive(true);

        if (playerController != null)
            playerController.SetMovementEnabled(false);

        if (InputBlockManager.Instance != null)
            InputBlockManager.Instance.BlockInput();

        ShowMessage(messages[currentMessageIndex]);
    }

    void ShowMessage(string message)
    {
        dialogueText.text = "";
        canPressNext = false;

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

        canPressNext = true;
    }

    void OnNextPressed()
    {
        currentMessageIndex++;

        if (currentMessageIndex < messages.Count)
        {
            ShowMessage(messages[currentMessageIndex]);
        }
        else
        {
            dialoguePanel.SetActive(false);
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
        acceptQuestButton.onClick.RemoveAllListeners();

        acceptQuestButton.onClick.AddListener(() =>
        {
            questPanel.SetActive(false);

            if (QuestManager.Instance != null)
                QuestManager.Instance.AddQuest(questName, 1);

            if (InputBlockManager.Instance != null)
                InputBlockManager.Instance.UnblockInput();

            SpawnQuestMarkerAtNPC();
        });
    }

    void SpawnQuestMarkerAtNPC()
    {
        npcTarget = GameObject.FindGameObjectWithTag(npcTag);
        if (npcTarget == null)
        {
            Debug.LogError($"❌ Không tìm thấy NPC có tag {npcTag} trong scene!");
            return;
        }

        // ✅ Xoá marker cũ nếu có
        if (currentMarkerInstance != null)
            Destroy(currentMarkerInstance);

        if (questMarkerPrefab != null)
        {
            Vector3 markerPos = npcTarget.transform.position;
            markerPos.y += markerHeightOffset;
            currentMarkerInstance = Instantiate(questMarkerPrefab, markerPos, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("⚠️ Chưa gán prefab cho questMarkerPrefab trong Inspector!");
        }

        // ✅ Cho phép người chơi di chuyển
        if (playerController != null)
            playerController.SetMovementEnabled(true);
    }
}
