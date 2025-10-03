using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;
    public BaseStats baseStats;
    public float maxHealth;
    public float maxMana;
    public float Attack;
    public float Defense;
    public float currentHealth;
    public float currentMana;

    void Start()
    {
        CheckedStats();
        Instance = this;
        currentHealth = baseStats.BHealth;
        currentMana = baseStats.BMana;

        UIStatsManager.Instance.UpdateHealth(currentHealth, baseStats.BHealth);
        UIStatsManager.Instance.UpdateMana(currentMana, baseStats.BMana);

        StartCoroutine(RegenerateMana());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H)) // test damage
        {
            TakeDamage(10f);

        }
        if (Input.GetKeyDown(KeyCode.M)) // test use mana
        {
            UseMana(5f);
        }
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
        UIStatsManager.Instance.UpdateHealth(currentHealth, maxMana);
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
}
