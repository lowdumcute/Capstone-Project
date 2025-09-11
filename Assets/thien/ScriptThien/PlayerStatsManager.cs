using UnityEngine;
using System.Collections;

public class PlayerStatsManager : MonoBehaviour
{
    public static PlayerStatsManager Instance;
    /// <summary>
    /// hệ thống quản lý chỉ số của nhân vật (Player Stats Manager)
    /// </summary>
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;   // Máu tối đa
    [SerializeField] private float currentHealth;      // Máu hiện tại

    [Header("Mana Settings")]
    [SerializeField] private float maxMana = 100f;     // Mana tối đa
    [SerializeField] private float currentMana;        // Mana hiện tại
    [SerializeField] private float manaRegenRate = 1f; // Tốc độ hồi mana (số mana hồi mỗi phút)
    private float manaRegenTimer = 0f;                 // Bộ đếm thời gian để tính hồi mana

    [Header("Skill Mana Costs")]
    [SerializeField] private float skill1ManaCost = 20f; // Lượng mana tốn cho skill 1
    [SerializeField] private float skill2ManaCost = 40f; // Lượng mana tốn cho skill 2

   
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public float CurrentMana => currentMana;
    public float MaxMana => maxMana;

    private void Awake()
    {
        // Thiết lập Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject); // Nếu đã có Instance khác, thì phá hủy object này
    }

    private void Start()
    {
        // Khởi tạo máu và mana bằng giá trị tối đa
        currentHealth = maxHealth;
        currentMana = maxMana;

        // Cập nhật UI lần đầu
        UpdateUI();
    }

    private void Update()
    {
        // Gọi hàm hồi mana liên tục mỗi frame
        HandleManaRegeneration();
    }

    /// <summary>
    /// Xử lý hồi mana theo thời gian.
    /// Mỗi 60 giây sẽ cộng thêm lượng mana bằng "manaRegenRate".
    /// </summary>
    private void HandleManaRegeneration()
    {
        manaRegenTimer += Time.deltaTime; // Tăng bộ đếm theo thời gian thực

        // Nếu vượt qua ngưỡng 60 giây / manaRegenRate => hồi mana
        if (manaRegenTimer >= 60f / manaRegenRate)
        {
            manaRegenTimer = 0f;      // Reset bộ đếm
            AddMana(manaRegenRate);   // Cộng thêm mana
        }
    }

    /// <summary>
    /// Cố gắng dùng skill 1 (kiểm tra đủ mana chưa).
    /// </summary>
    public bool UseSkill1()
    {
        if (CanUseSkill(skill1ManaCost)) // Nếu đủ mana
        {
            ConsumeMana(skill1ManaCost); // Trừ mana
            return true;                 // Cho phép dùng skill
        }
        return false; // Không đủ mana
    }

    /// <summary>
    /// Cố gắng dùng skill 2 (kiểm tra đủ mana chưa).
    /// </summary>
    public bool UseSkill2()
    {
        if (CanUseSkill(skill2ManaCost)) // Nếu đủ mana
        {
            ConsumeMana(skill2ManaCost); // Trừ mana
            return true;
        }
        return false;
    }

    /// <summary>
    /// Kiểm tra xem có đủ mana để dùng skill hay không.
    /// </summary>
    public bool CanUseSkill(float manaCost)
    {
        return currentMana >= manaCost;
    }

    /// <summary>
    /// Trừ mana khi dùng skill.
    /// </summary>
    public void ConsumeMana(float amount)
    {
        currentMana = Mathf.Max(0, currentMana - amount); // Đảm bảo không âm
        UpdateUI(); // Cập nhật UI
    }

    /// <summary>
    /// Hồi thêm mana (không vượt quá maxMana).
    /// </summary>
    public void AddMana(float amount)
    {
        currentMana = Mathf.Min(maxMana, currentMana + amount);
        UpdateUI();
    }

    /// <summary>
    /// Nhận sát thương và giảm máu.
    /// Nếu máu <= 0 thì gọi Die().
    /// </summary>
    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        UpdateUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Hồi máu (không vượt quá maxHealth).
    /// </summary>
    public void Heal(float healAmount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
        UpdateUI();
    }

    /// <summary>
    /// Hàm xử lý khi Player chết.
    /// Có thể thêm animation, game over, restart, v.v...
    /// </summary>
    private void Die()
    {
        Debug.Log("Player đã chết!");
    }

    /// <summary>
    /// Cập nhật UI cho máu và mana.
    /// </summary>
    private void UpdateUI()
    {
        if (UIStatsManager.Instance != null)
        {
            UIStatsManager.Instance.UpdateHealth(currentHealth, maxHealth);
            UIStatsManager.Instance.UpdateMana(currentMana, maxMana);
        }
    }

    // ================= CÁC HÀM SET/ADD GIÁ TRỊ TỪ BÊN NGOÀI =================

    /// <summary>
    /// Set máu tối đa mới, đảm bảo currentHealth không vượt quá max.
    /// </summary>
    public void SetMaxHealth(float value)
    {
        maxHealth = value;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        UpdateUI();
    }

    /// <summary>
    /// Set mana tối đa mới, đảm bảo currentMana không vượt quá max.
    /// </summary>
    public void SetMaxMana(float value)
    {
        maxMana = value;
        currentMana = Mathf.Min(currentMana, maxMana);
        UpdateUI();
    }

    /// <summary>
    /// Set tốc độ hồi mana (số mana hồi mỗi phút).
    /// </summary>
    public void SetManaRegenRate(float rate)
    {
        manaRegenRate = rate;
    }

    /// <summary>
    /// Cộng máu ngay lập tức (ví dụ: nhặt item).
    /// </summary>
    public void AddHealthInstant(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        UpdateUI();
    }

    /// <summary>
    /// Cộng mana ngay lập tức (ví dụ: uống bình mana).
    /// </summary>
    public void AddManaInstant(float amount)
    {
        currentMana = Mathf.Min(maxMana, currentMana + amount);
        UpdateUI();
    }
}
