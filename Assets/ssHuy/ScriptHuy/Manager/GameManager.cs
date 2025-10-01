using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;  // Singleton

    [Header("UI References")]
    [SerializeField] private GameObject inventoryUI; // Túi đồ

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
            inventoryUI.SetActive(false); // Mặc định tắt túi đồ
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) // Bấm phím I để bật/tắt
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        if (inventoryUI == null)
        {
            Debug.LogWarning("⚠ InventoryUI chưa gán trong GameManager!");
            return;
        }

        isInventoryOpen = !isInventoryOpen;
        inventoryUI.SetActive(isInventoryOpen);

        // Nếu mở túi đồ thì dừng game (tuỳ bạn)
        if (isInventoryOpen)
        {
            Time.timeScale = 0f; // Dừng game
        }
        else
        {
            Time.timeScale = 1f; // Chạy tiếp
        }
    }
}
