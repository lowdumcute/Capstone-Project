using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class SettingData
{
    public int resolutionIndex;
    public int displayMode;
}

public class SettingManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown displayModeDropdown;

    private Resolution[] resolutions;
    private string savePath;

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "settings.json");
    }

    private void Start()
    {
        SetupResolutionOptions();
        SetupDisplayModeOptions();

        // Gán listener chỉ 1 lần
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        displayModeDropdown.onValueChanged.AddListener(OnDisplayModeChanged);

        // Lần đầu load setting
        LoadSettings();
    }

    private void OnEnable()
    {
        // Mỗi lần panel bật lên, chỉ cần load lại JSON để cập nhật dropdown
        LoadSettings();
    }

    // 🖥️ Setup danh sách độ phân giải
    private void SetupResolutionOptions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        int currentResolutionIndex = 0;
        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = $"{resolutions[i].width} x {resolutions[i].height}";
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    // 🪟 Setup chế độ hiển thị
    private void SetupDisplayModeOptions()
    {
        displayModeDropdown.ClearOptions();
        displayModeDropdown.AddOptions(new List<string>
        {
            "Toàn màn hình",
            "Cửa sổ",
            "Không viền"
        });
    }

    // Khi chọn độ phân giải mới
    private void OnResolutionChanged(int index)
    {
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreenMode);
        SaveSettings();
    }

    // Khi chọn chế độ hiển thị mới
    private void OnDisplayModeChanged(int index)
    {
        switch (index)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
        }
        SaveSettings();
    }

    // 💾 Lưu dữ liệu vào file JSON
    private void SaveSettings()
    {
        SettingData data = new SettingData
        {
            resolutionIndex = resolutionDropdown.value,
            displayMode = displayModeDropdown.value
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"✅ Đã lưu cài đặt vào: {savePath}");
    }

    // 📂 Load dữ liệu từ file JSON
    private void LoadSettings()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("⚠️ Không tìm thấy file setting, dùng mặc định.");
            return;
        }

        string json = File.ReadAllText(savePath);
        SettingData data = JsonUtility.FromJson<SettingData>(json);

        resolutionDropdown.value = Mathf.Clamp(data.resolutionIndex, 0, resolutions.Length - 1);
        displayModeDropdown.value = Mathf.Clamp(data.displayMode, 0, 2);

        // Áp dụng lại
        OnResolutionChanged(resolutionDropdown.value);
        OnDisplayModeChanged(displayModeDropdown.value);

        resolutionDropdown.RefreshShownValue();
        displayModeDropdown.RefreshShownValue();

        Debug.Log("✅ Đã load cài đặt từ JSON!");
    }
}
