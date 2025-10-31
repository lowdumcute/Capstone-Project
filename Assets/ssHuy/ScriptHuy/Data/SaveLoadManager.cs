using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance;

    [Header("UI References")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject saveLoadPanel;
    [SerializeField] private GameObject SaveNamePanel;
    [SerializeField] private Transform saveListContainer;
    [SerializeField] private GameObject saveSlotPrefab;
    [SerializeField] private TMP_InputField newSaveNameInput;
    private string currentSaveName = null;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        RefreshSaveList();
    }

    /// Làm mới danh sách file lưu hiện có
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
    public void ResetSaveName()
    {
        currentSaveName = null;
    }
    public void SaveGame()
    {
        if (currentSaveName == null)
        {
            SaveNamePanel.SetActive(true);
        }
        else
        {
            SaveCurrentGame();
        }
    }
    /// Lưu game với tên mới (tạo slot lưu mới)

    public void CreateNewSave()
    {
        string saveName = newSaveNameInput.text.Trim();
        if (string.IsNullOrEmpty(saveName))
        {
            Debug.LogWarning(" Tên lưu trống!");
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
         // Lưu item vào PlayerData
        var inv = Inventory.instance;
        foreach (var item in inv.DataItem)
        {
            data.savedItems.Add(new ItemData(
                item.BaseStatsItem.itemName,
                item.amount,
                item.isEquippable
            ));
        }
        //  Lưu dữ liệu trang bị từ EquipmentManager
        var equipMgr = EquipmentManager.instance;
        foreach (var slot in equipMgr.equipmentSlots)
        {
            if (slot.currentItem != null)
            {
                data.equippedItemNames.Add(slot.currentItem.itemName);
            }
        }

        // Lưu lại chỉ số item từ EquipmentManager
        data.healthItem = equipMgr.HealthItem;
        data.manaItem = equipMgr.ManaItem;
        data.attackItem = equipMgr.AttackItem;
        data.armorItem = equipMgr.ArmorItem;       

        // Lưu tất cả quest
        foreach (var quest in QuestManager.Instance.allQuests)
        {
            QuestData qd = new QuestData();
            qd.questName = quest.questName;
            qd.questStatus = quest.questStatus.ToString();

            if (quest is QuestDefeatEnemy enemyQuest)
            {
                qd.isDefeatEnemyType = true;
                qd.currentValue = enemyQuest.currentValue;
            }

            data.allQuests.Add(qd);
        }

        // Lưu current quest
        if (QuestManager.Instance.currentQuest != null)
        {
            var current = QuestManager.Instance.currentQuest;
            QuestData qd = new QuestData();
            qd.questName = current.questName;
            qd.questStatus = current.questStatus.ToString();

            if (current is QuestDefeatEnemy enemyQuest)
            {
                qd.isDefeatEnemyType = true;
                qd.currentValue = enemyQuest.currentValue;
            }

            data.currentQuest = qd;
        }
        SaveSystem.SavePlayer(data, saveName);
        Debug.Log($" Đã lưu game và item: {saveName}");
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
            Debug.LogWarning($" Không thể load dữ liệu từ {saveName}!");
            yield break;
        }

        Debug.Log($" Load file {saveName}, scene: {data.currentScene}");

        AsyncOperation asyncLoad;

        if (SceneChangeManager.Instance != null)
        {
            if (saveLoadPanel != null)
                saveLoadPanel.SetActive(false);
            // Load scene nhưng giữ lại màn hình loading
            yield return SceneChangeManager.Instance.StartCoroutine(
                SceneChangeManager.Instance.LoadSceneAndWaitClose(data.currentScene)
            );

            // Cập nhật dữ liệu & dịch chuyển player trước khi đóng loading
            var dataMgr = GameManager.Instance.dataGameManager;
            dataMgr.changePlayerStats(data.role); // Cập nhật chỉ số cơ bản người chơi
            dataMgr.currentLevel = data.level;
            dataMgr.exp = data.exp;
            GameManager.Instance.Player.transform.position = data.position;
            currentSaveName = saveName;

            // Đợi các singleton khởi tạo xong trước khi equip
            float waitTime = 0f;
            while ((EquipmentManager.instance == null ||
                    PlayerStats.Instance == null ||
                    StatsUI.Instance == null ||
                    UIStatsManager.Instance == null) 
                && waitTime < 5f)
            {
                waitTime += Time.deltaTime;
                yield return null;
            }

            if (waitTime >= 5f)
                Debug.LogWarning(" Hết thời gian chờ — một trong các Singleton chưa được khởi tạo!");
                Debug.Log($"EquipmentManager: {(EquipmentManager.instance == null ? " null" : " ok")}");
                Debug.Log($"PlayerStats: {(PlayerStats.Instance == null ? " null" : " ok")}");
                Debug.Log($"StatsUI: {(StatsUI.Instance == null ? " null" : " ok")}");
                Debug.Log($"UIStatsManager: {(UIStatsManager.Instance == null ? " null" : " ok")}");

            // Nếu vẫn chưa đủ chắc, có thể chờ thêm một chút
            yield return new WaitForSeconds(0.2f);
            // Load lại item inventory
            var inv = Inventory.instance;
            foreach (var savedItem in data.savedItems)
            {
                // Tìm item tương ứng trong danh sách DataItem
                Item found = inv.FindItem(
                    inv.DataItem.FirstOrDefault(i => i.BaseStatsItem.itemName == savedItem.itemName)?.BaseStatsItem
                );

                if (found != null)
                {
                    found.amount = savedItem.amount;
                    found.isEquippable = savedItem.isEquipped;
                }
            }
            Debug.Log(" Inventory đã được khôi phục từ file lưu!");

            //  Load lại trang bị
            var equipMgr = EquipmentManager.instance;

            // Reset lại toàn bộ slot
            foreach (var slot in equipMgr.equipmentSlots)
                slot.currentItem = null;

            // Gán lại item theo danh sách lưu
            foreach (var itemName in data.equippedItemNames)
            {
                // Tìm BaseItem trùng tên trong Inventory.DataItem
                var baseItem = Inventory.instance.DataItem
                    .FirstOrDefault(i => i.BaseStatsItem.itemName == itemName)?.BaseStatsItem;

                if (baseItem != null)
                {
                    // Trang bị lại item
                    equipMgr.Equip(baseItem);
                }
            }

            // Khôi phục chỉ số item (đề phòng trường hợp Equip chưa cộng xong)
            equipMgr.HealthItem = data.healthItem;
            equipMgr.ManaItem = data.manaItem;
            equipMgr.AttackItem = data.attackItem;
            equipMgr.ArmorItem = data.armorItem;

            // Cập nhật lại UI
            UIStatsManager.Instance.UpdateHealth(PlayerStats.Instance.currentHealth, PlayerStats.Instance.maxHealth);
            UIStatsManager.Instance.UpdateMana(PlayerStats.Instance.currentMana, PlayerStats.Instance.maxMana);

            Debug.Log(" Đã khôi phục lại toàn bộ trang bị & chỉ số!");

            //  Cập nhật lại trạng thái nhiệm vụ đã lưu
            foreach (var qd in data.allQuests)
            {
                // Tìm quest gốc theo tên
                var quest = QuestManager.Instance.allQuests
                    .FirstOrDefault(q => q.questName == qd.questName);

                if (quest != null)
                {
                    quest.questStatus = Enum.Parse<QuestStatus>(qd.questStatus);

                    if (quest is QuestDefeatEnemy enemyQuest && qd.isDefeatEnemyType)
                    {
                        enemyQuest.currentValue = qd.currentValue;
                    }
                }
                else
                {
                    Debug.LogWarning($" Không tìm thấy quest tên '{qd.questName}' trong allQuests!");
                }
            }

            //  Khôi phục current quest
            if (data.currentQuest != null)
            {
                var qd = data.currentQuest;
                var currentQuest = QuestManager.Instance.allQuests
                    .FirstOrDefault(q => q.questName == qd.questName);

                if (currentQuest != null)
                {
                    currentQuest.questStatus = Enum.Parse<QuestStatus>(qd.questStatus);

                    if (currentQuest is QuestDefeatEnemy enemyQuest && qd.isDefeatEnemyType)
                        enemyQuest.currentValue = qd.currentValue;

                    QuestManager.Instance.currentQuest = currentQuest;
                }
                else
                {
                    Debug.LogWarning($" Không tìm thấy current quest '{qd.questName}' trong allQuests!");
                }
            }
            //  Giờ mới fade out
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

        Debug.Log($" Dữ liệu {saveName} đã được load thành công!");
    }
    public void SaveCurrentGame()
    {
        if (string.IsNullOrEmpty(currentSaveName))
        {
            Debug.LogWarning(" Chưa có file đang chơi, hãy lưu bằng 'CreateNewSave'!");
            return;
        }

        var dataMgr = GameManager.Instance.dataGameManager;
        PlayerData data = new PlayerData(
            dataMgr.playerStatsUsing.characterName,
            dataMgr.currentLevel,
            dataMgr.exp,
            GameManager.Instance.Player.transform.position,
            SceneManager.GetActiveScene().name,
            currentSaveName
        );
        // Lưu item vào PlayerData
        var inv = Inventory.instance;
        foreach (var item in inv.DataItem)
        {
            data.savedItems.Add(new ItemData(
                item.BaseStatsItem.itemName,
                item.amount,
                item.isEquippable
            ));
        }
        // Lưu dữ liệu trang bị từ EquipmentManager
        var equipMgr = EquipmentManager.instance;
        foreach (var slot in equipMgr.equipmentSlots)
        {
            if (slot.currentItem != null)
            {
                data.equippedItemNames.Add(slot.currentItem.itemName);
            }
        }

        // Lưu lại chỉ số item từ EquipmentManager
        data.healthItem = equipMgr.HealthItem;
        data.manaItem = equipMgr.ManaItem;
        data.attackItem = equipMgr.AttackItem;
        data.armorItem = equipMgr.ArmorItem;
        // Lưu tất cả quest
        foreach (var quest in QuestManager.Instance.allQuests)
        {
            QuestData qd = new QuestData();
            qd.questName = quest.questName;
            qd.questStatus = quest.questStatus.ToString();

            if (quest is QuestDefeatEnemy enemyQuest)
            {
                qd.isDefeatEnemyType = true;
                qd.currentValue = enemyQuest.currentValue;
            }

            data.allQuests.Add(qd);
        }

        // Lưu current quest
        if (QuestManager.Instance.currentQuest != null)
        {
            var current = QuestManager.Instance.currentQuest;
            QuestData qd = new QuestData();
            qd.questName = current.questName;
            qd.questStatus = current.questStatus.ToString();

            if (current is QuestDefeatEnemy enemyQuest)
            {
                qd.isDefeatEnemyType = true;
                qd.currentValue = enemyQuest.currentValue;
            }

            data.currentQuest = qd;
        }
        SaveSystem.SavePlayer(data, currentSaveName);
        Debug.Log($" Đã ghi đè file {currentSaveName}");
    }
    public void DeletedFile(string saveName)
    {
        SaveSystem.DeleteSave(saveName);
        RefreshSaveList();
    }
    // UI Panels
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
