using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    public BaseStats baseStats;
    public float currentHealth;
    public float currentMana;

    void Start()
    {
        currentHealth = baseStats.Health;
        currentMana = baseStats.Mana;

        UIStatsManager.Instance.UpdateHealth(currentHealth, baseStats.Health);
        UIStatsManager.Instance.UpdateMana(currentMana, baseStats.Mana);

        StartCoroutine(RegenerateMana());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H)) // Simulate taking damage
        {
            TakeDamage(10f);
        }
        if (Input.GetKeyDown(KeyCode.M)) // Simulate using mana
        {
            UseMana(5f);
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        UIStatsManager.Instance.UpdateHealth(currentHealth, baseStats.Health);
    }

    public void UseMana(float amount)
    {
        if (currentMana < amount)
        {
            Debug.LogWarning("Not enough mana!");
            return;
        }
        currentMana -= amount;
        UIStatsManager.Instance.UpdateMana(currentMana, baseStats.Mana);
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
}
