using UnityEngine;

public class PetController : MonoBehaviour
{
    public Transform player;
    public float followDistance = 1f;
    public float moveSpeed = 5f;
    public float rotationSpeed = 5f;
    public float attackRange = 10f;
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    public float fireRate = 1f;
    public float lifetime = 10f;

    private float fireCooldown;
    private float lifeTimer;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        lifeTimer = lifetime;
    }

    private void Update()
    {
        HandleFollow();
        HandleAttack();
        HandleLifetime();
    }

    private void HandleFollow()
    {
        if (player == null) return;

        Vector3 targetPos = player.position - player.forward * followDistance;
        targetPos.y = player.position.y;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * moveSpeed);
    }

    private void HandleAttack()
    {
        fireCooldown -= Time.deltaTime;

        GameObject target = FindNearestEnemy();
        if (target != null)
        {
            // Xoay về enemy
            Vector3 dir = (target.transform.position - transform.position).normalized;
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * rotationSpeed);

            if (fireCooldown <= 0f)
            {
                Fire(target);
                fireCooldown = 1f / fireRate;
            }
        }
        else if (player != null)
        {
            // ✅ Nếu không có enemy → xoay về phía player
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            Quaternion lookRot = Quaternion.LookRotation(dirToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * rotationSpeed);
        }
    }

    private void Fire(GameObject target)
    {
        if (bulletPrefab == null || firePoint == null || target == null) return;
        animator.SetTrigger("Attack");
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Vector3 direction = (target.transform.position - firePoint.position).normalized;

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction * bulletSpeed;
        }
    }

    private GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist <= attackRange && dist < minDist)
            {
                minDist = dist;
                nearest = enemy;
            }
        }

        return nearest;
    }

    private void HandleLifetime()
    {
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
