using UnityEngine;


[RequireComponent(typeof(Animator))]
public class EnemyHealth : MonoBehaviour
{
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
    void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }
    public void TakeDamage(float amount)
    {
       
        if (isDead) return;
        if (Time.time < lastHitTime + invulnerabilityTimeAfterHit) return; // tránh trúng đòn quá nhanh
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
        animator.SetTrigger("Die");
        CanvasHealthBar.SetActive(false);
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
        EnemyController enemyCol = GetComponent<EnemyController>();
        if (enemyCol != null) enemyCol.enabled = false;
        this.gameObject.tag = "Default";
        Debug.Log("Death");
    }
}
