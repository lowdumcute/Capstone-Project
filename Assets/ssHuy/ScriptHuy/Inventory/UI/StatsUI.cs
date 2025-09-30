using UnityEngine;
using TMPro;

public class StatsUI : MonoBehaviour
{
    public static StatsUI Instance { get; private set; }
    [SerializeField] private PlayerStats playerStats;
    private BaseStats baseStats;
    private TextMeshProUGUI[] statTexts;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        baseStats = playerStats.baseStats;

        // Lấy tất cả TextMeshPro con (theo thứ tự hierarchy trong Inspector)
        statTexts = GetComponentsInChildren<TextMeshProUGUI>();
    }

    public void Update()
    {
        UpdateStatsUI();
    }
    public void UpdateStatsUI()
    {
        if (statTexts == null || statTexts.Length < 4) return;

        statTexts[0].text = "Attack: " + playerStats.Attack;
        statTexts[1].text = "Armor: " + playerStats.Armor;

        statTexts[2].text = $"Health: " + playerStats.currentHealth + "/" + playerStats.maxHealth;
        statTexts[3].text = "Mana: " + playerStats.currentMana + "/" + playerStats.maxMana;
    }
}
