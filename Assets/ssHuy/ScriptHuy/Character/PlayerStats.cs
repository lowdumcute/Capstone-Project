using UnityEngine;
using System.Collections;
using MaykerStudio.Demo;
using System.Diagnostics.Contracts;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;
    public BaseStats baseStats;
    public float maxHealth;
    public float maxMana;
    public float Attack;
    public float Armor;
    public float currentHealth;
    public float currentMana;

    void Start()
    {
        CheckedStats();
        Instance = this;
        currentHealth = baseStats.Health;
        currentMana = baseStats.Mana;

        UIStatsManager.Instance.UpdateHealth(currentHealth, baseStats.Health);
        UIStatsManager.Instance.UpdateMana(currentMana, baseStats.Mana);

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
        StatsUI.Instance.UpdateStatsUI();
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

            if (currentMana < baseStats.Mana)
            {
                currentMana += 1f;
                currentMana = Mathf.Min(currentMana, baseStats.Mana);
                UIStatsManager.Instance.UpdateMana(currentMana, baseStats.Mana);
            }
        }
    }
    public void CheckedStats()
    {
        Attack = baseStats.Attack + EquipmentManager.instance.AttackItem;
        Armor = baseStats.Armor + EquipmentManager.instance.ArmorItem;
        maxHealth = baseStats.Health + EquipmentManager.instance.HealthItem;
        maxMana = baseStats.Mana + EquipmentManager.instance.ManaItem;
    }
}
