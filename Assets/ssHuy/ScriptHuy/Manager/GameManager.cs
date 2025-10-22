using UnityEngine;
using System.IO;

public class GameManager : MonoBehaviour
{
    [SerializeField] public DataGameManager dataGameManager;
    public static GameManager Instance; 

    [Header("UI References")]
    [SerializeField] private InventoryUI inventoryUI; // Tham chiếu trực tiếp tới InventoryUI

    private bool isInventoryOpen = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (inventoryUI != null)
            inventoryUI.gameObject.SetActive(false); // Tắt UI lúc đầu
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) // Bấm phím I để bật/tắt
        {
            ToggleInventory();
        }
    }

    // 🔹 Lưu dữ liệu vào file JSON
    public void SaveProgress()
    {
        GameData data = new GameData();
        data.level = dataGameManager.currentLevel;

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savegame.json", json);
        Debug.Log("Đã lưu " + dataGameManager);
    }

    // 🔹 Tải dữ liệu từ file JSON
    public void LoadProgress()
    {
        string filePath = Application.persistentDataPath + "/savegame.json";

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            GameData data = JsonUtility.FromJson<GameData>(json);

            dataGameManager.currentLevel = data.level;
            dataGameManager.Position = data.position;

            foreach (var role in dataGameManager.AllRoleStats)
            {
                if (role.name == data.Role)
                {
                    dataGameManager.playerStatsUsing = role;
                    break;
                }
            }
        }
        else
        {
            dataGameManager.currentLevel = 1;
        }
    }

    // 🔹 Bật/tắt túi đồ
    public void ToggleInventory()
    {
        if (inventoryUI == null)
        {
            Debug.LogWarning("⚠ InventoryUI chưa gán trong GameManager!");
            return;
        }

        isInventoryOpen = !isInventoryOpen;
        inventoryUI.gameObject.SetActive(isInventoryOpen);

        if (isInventoryOpen)
        {

            // Gọi cập nhật UI khi bật
            inventoryUI.UpdateUI();
        }
        else
        {
            Time.timeScale = 1f; // Chạy tiếp
        }
    }

    // 🔹 Gán chỉ số người chơi hiện tại
    public void AddPlayerStats(BaseStats playerStats)
    {
        dataGameManager.playerStatsUsing = playerStats;
    }
}
