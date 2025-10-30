using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance;

    [Header("UI References")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject saveLoadPanel;
    [SerializeField] private Transform saveListContainer;
    [SerializeField] private GameObject saveSlotPrefab;
    [SerializeField] private TMP_InputField newSaveNameInput;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        RefreshSaveList();
    }

    /// <summary>
    /// Làm mới danh sách file lưu hiện có
    /// </summary>
    public void RefreshSaveList()
    {
        // Xóa slot cũ
        foreach (Transform child in saveListContainer)
            Destroy(child.gameObject);

        // Lấy danh sách file lưu
        List<string> saveFiles = SaveSystem.GetAllSaveFiles();

        foreach (string fileName in saveFiles)
        {
            GameObject slot = Instantiate(saveSlotPrefab, saveListContainer);
            slot.GetComponentInChildren<TMP_Text>().text = fileName;

            // --- Gắn nút Load ---
            Button loadBtn = slot.transform.Find("LoadButton").GetComponent<Button>();
            loadBtn.onClick.RemoveAllListeners(); // tránh bị cộng dồn listener
            loadBtn.onClick.AddListener(() => LoadGame(fileName));

            // --- Gắn nút Delete ---
            Button deleteBtn = slot.transform.Find("DeleteButton").GetComponent<Button>();
            deleteBtn.onClick.RemoveAllListeners();
            deleteBtn.onClick.AddListener(() => DeletedFile(fileName));
        }
    }

    /// Lưu game với tên mới (tạo slot lưu mới)

    public void CreateNewSave()
    {
        string saveName = newSaveNameInput.text.Trim();
        if (string.IsNullOrEmpty(saveName))
        {
            Debug.LogWarning("⚠️ Tên lưu trống!");
            return;
        }
        // Lưu dữ liệu hiện tại
        var dataMgr = GameManager.Instance.dataGameManager;
        PlayerData data = new PlayerData(
            dataMgr.playerStatsUsing.characterName,
            dataMgr.currentLevel,
            dataMgr.exp,
            GameManager.Instance.Player.transform.position,
            SceneManager.GetActiveScene().name,
            saveName
        );

        SaveSystem.SavePlayer(data, saveName);
        Debug.Log($"✅ Đã lưu game: {saveName}");

        RefreshSaveList();
        newSaveNameInput.text = "";
    }

    public void LoadGame(string saveName)
    {
        StartCoroutine(LoadGameRoutine(saveName));
    }

    private IEnumerator LoadGameRoutine(string saveName)
    {
        PlayerData data = SaveSystem.LoadPlayer(saveName);
        if (data == null)
        {
            Debug.LogWarning($"❌ Không thể load dữ liệu từ {saveName}!");
            yield break;
        }

        Debug.Log($"🔄 Load file {saveName}, scene: {data.currentScene}");

        AsyncOperation asyncLoad;

        if (SceneChangeManager.Instance != null)
        {
            if (saveLoadPanel != null)
                saveLoadPanel.SetActive(false);
            // Load scene nhưng giữ lại màn hình loading
            yield return SceneChangeManager.Instance.StartCoroutine(
                SceneChangeManager.Instance.LoadSceneAndWaitClose(data.currentScene)
            );

            // ✅ Cập nhật dữ liệu & dịch chuyển player trước khi đóng loading
            var dataMgr = GameManager.Instance.dataGameManager;
            dataMgr.changePlayerStats(data.role); // Cập nhật chỉ số cơ bản người chơi
            dataMgr.currentLevel = data.level;
            dataMgr.exp = data.exp;
            GameManager.Instance.Player.transform.position = data.position;

            // ⏳ Giờ mới fade out
            yield return SceneChangeManager.Instance.StartCoroutine(
                SceneChangeManager.Instance.CloseLoadingScreen()
            );

        }
        else
        {
            asyncLoad = SceneManager.LoadSceneAsync(data.currentScene);
            while (!asyncLoad.isDone)
                yield return null;
        }

        Debug.Log($"✅ Dữ liệu {saveName} đã được load thành công!");
    }
    
    public void DeletedFile(string saveName)
    {
        SaveSystem.DeleteSave(saveName);
        RefreshSaveList();
    }
    public void OpenLoadPanel()
    {
        saveLoadPanel.SetActive(true);
        RefreshSaveList();
    }

    public void ToggleSettingPanel()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }
}
