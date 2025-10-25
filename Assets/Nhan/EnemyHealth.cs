using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Info")]
    [SerializeField] private string enemyName; // tên quái
    public string EnemyName => enemyName; // public getter cho quest

    [Header("Stats")]
    public float maxHealth = 100f;
    public float currentHealth;

    public Animator animator;
    public bool destroyOnDeath = true;
    public float invulnerabilityTimeAfterHit = 0.15f; // i-frames nhỏ

    private Rigidbody rb;
    private bool isDead = false;
    private float lastHitTime = -99f;

    [Header("UI")]
    public EnemyHealthBar hbController;
    public GameObject CanvasHealthBar;

    public float damage = 50f;

    void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(float amount)
    {
        AudioManager.Instance.PlayHitSound();
        if (isDead) return;
        if (Time.time < lastHitTime + invulnerabilityTimeAfterHit) return;

        currentHealth -= amount;

        // Cập nhật thanh máu
        if (hbController != null)
        {
            Debug.Log($"Dame Recive: {amount}");
            hbController.UpdateHealthBar(amount);
        }

        if (currentHealth <= 0f)
            Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        animator.SetTrigger("Die");
        if (CanvasHealthBar != null)
            CanvasHealthBar.SetActive(false);

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        EnemyController enemyCol = GetComponent<EnemyController>();
        if (enemyCol != null) enemyCol.enabled = false;

        this.gameObject.tag = "Default";

        // ✅ Gọi quest nếu có
        if (QuestManager.Instance.currentQuest is QuestDefeatEnemy defeatQuest)
        {
            defeatQuest.OnEnemyDefeated(enemyName);
            QuestManager.Instance.UpdateUI();
        }

        Debug.Log($"{enemyName} Death");

        if (destroyOnDeath)
            Destroy(gameObject, 2f); // tùy chọn destroy sau animation
    }
}
