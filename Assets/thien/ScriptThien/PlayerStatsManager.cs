using UnityEngine;
using System.Collections;

public class PlayerStatsManager : MonoBehaviour
{
    public static PlayerStatsManager Instance;

    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("Mana Settings")]
    [SerializeField] private float maxMana = 100f;
    [SerializeField] private float currentMana;
    [SerializeField] private float manaRegenRate = 1f; // Mana hồi mỗi phút
    private float manaRegenTimer = 0f;

    [Header("Skill Mana Costs")]
    [SerializeField] private float skill1ManaCost = 20f;
    [SerializeField] private float skill2ManaCost = 40f;

    // Properties để các script khác truy cập
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public float CurrentMana => currentMana;
    public float MaxMana => maxMana;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // Khởi tạo giá trị máu và mana
        currentHealth = maxHealth;
        currentMana = maxMana;

        // Cập nhật UI lần đầu
        UpdateUI();
    }

    private void Update()
    {
        // Hồi mana theo thời gian
        HandleManaRegeneration();
    }

    private void HandleManaRegeneration()
    {
        manaRegenTimer += Time.deltaTime;

        // Mỗi 60 giây hồi mana theo rate
        if (manaRegenTimer >= 60f / manaRegenRate)
        {
            manaRegenTimer = 0f;
            AddMana(manaRegenRate);
        }
    }

    public bool UseSkill1()
    {
        if (CanUseSkill(skill1ManaCost))
        {
            ConsumeMana(skill1ManaCost);
            return true;
        }
        return false;
    }

    public bool UseSkill2()
    {
        if (CanUseSkill(skill2ManaCost))
        {
            ConsumeMana(skill2ManaCost);
            return true;
        }
        return false;
    }

    public bool CanUseSkill(float manaCost)
    {
        return currentMana >= manaCost;
    }

    public void ConsumeMana(float amount)
    {
        currentMana = Mathf.Max(0, currentMana - amount);
        UpdateUI();
    }

    public void AddMana(float amount)
    {
        currentMana = Mathf.Min(maxMana, currentMana + amount);
        UpdateUI();
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        UpdateUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float healAmount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
        UpdateUI();
    }

    private void Die()
    {
        // Xử lý khi player chết
        Debug.Log("Player đã chết!");
        // Có thể thêm animation, restart game, etc.
    }

    private void UpdateUI()
    {
        if (UIStatsManager.Instance != null)
        {
            UIStatsManager.Instance.UpdateHealth(currentHealth, maxHealth);
            UIStatsManager.Instance.UpdateMana(currentMana, maxMana);
        }
    }

    // Các hàm để thiết lập giá trị từ bên ngoài
    public void SetMaxHealth(float value)
    {
        maxHealth = value;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        UpdateUI();
    }

    public void SetMaxMana(float value)
    {
        maxMana = value;
        currentMana = Mathf.Min(currentMana, maxMana);
        UpdateUI();
    }

    public void SetManaRegenRate(float rate)
    {
        manaRegenRate = rate;
    }

    // Hàm để thêm máu/mana ngay lập tức (cho item, buff, etc.)
    public void AddHealthInstant(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        UpdateUI();
    }

    public void AddManaInstant(float amount)
    {
        currentMana = Mathf.Min(maxMana, currentMana + amount);
        UpdateUI();
    }
}