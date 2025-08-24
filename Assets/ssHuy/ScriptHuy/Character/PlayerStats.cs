using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private CharacterBaseStats baseStats;
    public float currentHealth;
    public float currentMana;

    void Start()
    {
        currentHealth = baseStats.maxHealth;
        currentMana = baseStats.maxMana;

        UIStatsManager.Instance.UpdateHealth(currentHealth, baseStats.maxHealth);
        UIStatsManager.Instance.UpdateMana(currentMana, baseStats.maxMana);

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
        UIStatsManager.Instance.UpdateHealth(currentHealth, baseStats.maxHealth);
    }

    public void UseMana(float amount)
    {
        if (currentMana < amount)
        {
            Debug.LogWarning("Not enough mana!");
            return;
        }
        currentMana -= amount;
        UIStatsManager.Instance.UpdateMana(currentMana, baseStats.maxMana);
    }

    private IEnumerator RegenerateMana()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (currentMana < baseStats.maxMana)
            {
                currentMana += 1f;
                currentMana = Mathf.Min(currentMana, baseStats.maxMana);
                UIStatsManager.Instance.UpdateMana(currentMana, baseStats.maxMana);
            }
        }
    }
}
