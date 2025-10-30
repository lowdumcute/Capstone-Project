using UnityEngine;
using System.IO;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField] public DataGameManager dataGameManager;
    public static GameManager Instance;

    [Header("UI References")]
    [SerializeField] private InventoryUI inventoryUI; // Tham chiếu trực tiếp tới InventoryUI
    public GameObject Player;

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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SaveLoadManager.Instance.ToggleSettingPanel();
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
