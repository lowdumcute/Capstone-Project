using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UILevelManager : MonoBehaviour
{
    public static UILevelManager Instance;

    [Header("UI References")]
    public Image expBar;                 // Thanh EXP (fill)
    public TextMeshProUGUI levelText;    // Hiển thị Level

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Cập nhật thanh exp và cấp hiện tại
    /// </summary>
    public void UpdateExpUI(int currentExp, int expToNextLevel, int currentLevel)
    {
        if (expBar != null)
            expBar.fillAmount = (float)currentExp / expToNextLevel;

        if (levelText != null)
            levelText.text = $"Lv. {currentLevel}";
    }
}
