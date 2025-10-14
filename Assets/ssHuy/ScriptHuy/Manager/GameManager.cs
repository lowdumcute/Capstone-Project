using UnityEngine;
using System.IO;
public class GameManager : MonoBehaviour
{
    [SerializeField] public DataGameManager dataGameManager;
    public static GameManager Instance; 

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
    // Lưu dữ liệu vào file JSON
    public void SaveProgress()
    {
        GameData data = new GameData();
        data.level = dataGameManager.currentLevel; // lưu cấp độ
        //data.Role = dataGameManager.playerStatsUsing.NameRole; // lưu tên role
        //data.position= GamePlayManager.Instance.Player.transform.position; // lưu vị trí
        //data.SceneSave = SceneManager.GetActiveScene().name; //
        //data.level = PlayerLevel.Instance.currentExp;

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savegame.json", json);
        Debug.Log ("Đã lưu" + dataGameManager);
    }

    public void LoadProgress()
    {
        string filePath = Application.persistentDataPath + "/savegame.json";

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            GameData data = JsonUtility.FromJson<GameData>(json);
            
            // Cập nhật ScriptableObject với dữ liệu từ JSON
            dataGameManager.currentLevel = data.level;
            dataGameManager.Position = data.position;
            foreach (var role in dataGameManager.AllRoleStats)
            {
            if (role.name == data.Role) // So sánh với tên đã lưu
            {
                dataGameManager.playerStatsUsing = role;
                break;
            }
            }
        }
        else
        {
            // Nếu không có file lưu, khởi tạo với giá trị mặc định (ví dụ, cấp độ 1)
            dataGameManager.currentLevel = 1;
            //GamePlayManager.Instance.Player.GetComponent<CharacterController>().enabled = true;
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
    public void AddPlayerStats(BaseStats playerStats)
    {
        dataGameManager.playerStatsUsing = playerStats;
    }
}
