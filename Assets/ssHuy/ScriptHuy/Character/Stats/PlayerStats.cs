using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    [Header("Player Stats")]
    public static PlayerStats Instance;
    public BaseStats baseStats;
    public float maxHealth;
    public float maxMana;
    public float Attack;
    public float Defense;
    public float currentHealth;
    public float currentMana;

    [Header("Exp & Level")]
    public int currentLevel = 1;
    public int currentExp = 0;
    public int expToNextLevel = 100; // EXP cần để lên cấp đầu tiên

    void Start()
    {
        Instance = this;
        CheckedStats();
        currentHealth = baseStats.BHealth;
        currentMana = baseStats.BMana;

        UIStatsManager.Instance.UpdateHealth(currentHealth, baseStats.BHealth);
        UIStatsManager.Instance.UpdateMana(currentMana, baseStats.BMana);
        UILevelManager.Instance.UpdateExpUI(currentExp, expToNextLevel, currentLevel);
        StartCoroutine(RegenerateMana());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H)) // test damage
            TakeDamage(10f);

        if (Input.GetKeyDown(KeyCode.M)) // test use mana
            UseMana(5f);

        if (Input.GetKeyDown(KeyCode.X)) // test exp
            AddExp(50);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        UIStatsManager.Instance.UpdateHealth(currentHealth, maxHealth);
        StatsUI.Instance.UpdateStatsUI();
    }

    public void UseMana(float amount)
    {
        if (currentMana < amount)
        {
            Debug.LogWarning("Not enough mana!");
            return;
        }

        currentMana -= amount;
        UIStatsManager.Instance.UpdateMana(currentMana, maxMana);
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        UIStatsManager.Instance.UpdateHealth(currentHealth, maxHealth);
        StatsUI.Instance.UpdateStatsUI();
    }

    private IEnumerator RegenerateMana()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (currentMana < baseStats.BMana)
            {
                currentMana += 1f;
                currentMana = Mathf.Min(currentMana, baseStats.BMana);
                UIStatsManager.Instance.UpdateMana(currentMana, baseStats.BMana);
            }
        }
    }

    public void CheckedStats()
    {
        Attack = baseStats.BAttack + EquipmentManager.instance.AttackItem;
        Defense = baseStats.BDefense + EquipmentManager.instance.ArmorItem;
        maxHealth = baseStats.BHealth + EquipmentManager.instance.HealthItem;
        maxMana = baseStats.BMana + EquipmentManager.instance.ManaItem;
    }

    // ✅ Thêm EXP và xử lý lên cấp
    public void AddExp(int amount)
    {
        currentExp += amount;
        Debug.Log($"Gained {amount} EXP. Total: {currentExp}/{expToNextLevel}");
        UILevelManager.Instance.UpdateExpUI(currentExp, expToNextLevel, currentLevel);

        while (currentExp >= expToNextLevel)
        {
            currentExp -= expToNextLevel;
            LevelUp();
        }
    }

    // ✅ Hàm xử lý lên cấp
    private void LevelUp()
    {
        currentLevel++;
        expToNextLevel *= 2; // EXP cần cho cấp kế tiếp x2
        baseStats.BHealth += 10;  // tăng chỉ số cơ bản mỗi level
        baseStats.BMana += 5;
        baseStats.BAttack += 2;
        baseStats.BDefense += 1;

        CheckedStats();
        UILevelManager.Instance.UpdateExpUI(currentExp, expToNextLevel, currentLevel);

        // hồi đầy máu và mana khi lên cấp
        currentHealth = maxHealth;
        currentMana = maxMana;

        UIStatsManager.Instance.UpdateHealth(currentHealth, maxHealth);
        UIStatsManager.Instance.UpdateMana(currentMana, maxMana);
        StatsUI.Instance.UpdateStatsUI();

        Debug.Log($"🎉 Level Up! Now Level {currentLevel}. Next Level requires {expToNextLevel} EXP.");
    }
}
