using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyHealth : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 100f;
    public bool destroyOnDeath = true;
    public GameObject deathVFX;
    public float invulnerabilityTimeAfterHit = 0.15f; // i-frames nhỏ

    [Header("UI")]
    public GameObject healthBarPrefab; // prefab World Space canvas
    public Vector3 healthBarOffset = new Vector3(0f, 2.2f, 0f);

    private float currentHealth;
    private Rigidbody rb;
    private bool isDead = false;
    private float lastHitTime = -99f;

    private GameObject hbInstance;
    private EnemyHealthBar hbController;

    void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();

        // Tạo thanh máu nếu có prefab
        if (healthBarPrefab != null)
        {
            hbInstance = Instantiate(healthBarPrefab, transform.position + healthBarOffset, Quaternion.identity, transform);
            hbInstance.transform.localPosition = healthBarOffset;
            hbController = hbInstance.GetComponent<EnemyHealthBar>();
            if (hbController != null)
                hbController.SetMaxHealth(maxHealth);
        }
    }

    public void TakeDamage(float amount, Vector3 hitPoint, Vector3 knockbackForce)
    {
        if (isDead) return;
        if (Time.time < lastHitTime + invulnerabilityTimeAfterHit) return; // tránh trúng đòn quá nhanh

        lastHitTime = Time.time;
        currentHealth -= amount;

        OnHit(knockbackForce);

        // Cập nhật thanh máu
        if (hbController != null)
            hbController.SetHealth(currentHealth);

        if (currentHealth <= 0f)
            Die();
    }

    void OnHit(Vector3 knockbackForce)
    {
        // Tác động vật lý (nếu có)
        if (rb != null && !rb.isKinematic)
            rb.AddForce(knockbackForce, ForceMode.Impulse);
    }

    void Die()
    {
        isDead = true;

        if (deathVFX != null)
            Instantiate(deathVFX, transform.position, Quaternion.identity);

        // Tắt collider
        var colliders = GetComponentsInChildren<Collider>();
        foreach (var c in colliders)
            c.enabled = false;

        // Ẩn thanh máu
        if (hbInstance != null)
            hbInstance.SetActive(false);

        if (destroyOnDeath)
            Destroy(gameObject, 0.5f);
    }
}
