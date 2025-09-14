using UnityEngine;
using TMPro;

public class StatsUI : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    private BaseStats baseStats;
    private TextMeshProUGUI[] statTexts;

    private void Start()
    {
        baseStats = playerStats.baseStats;

        // Lấy tất cả TextMeshPro con (theo thứ tự hierarchy trong Inspector)
        statTexts = GetComponentsInChildren<TextMeshProUGUI>();

        UpdateStatsUI();
    }

    public void UpdateStatsUI()
    {
        if (statTexts == null || statTexts.Length < 4) return;

        statTexts[0].text = "Attack: " + baseStats.Attack;
        statTexts[1].text = "Armor: " + baseStats.Armor;
        statTexts[2].text = "Health: " + baseStats.Health;
        statTexts[3].text = "Mana: " + baseStats.Mana;
    }
}
