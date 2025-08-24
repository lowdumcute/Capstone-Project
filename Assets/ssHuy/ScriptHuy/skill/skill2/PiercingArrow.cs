using UnityEngine;

public class PiercingArrow : MonoBehaviour
{
    private SkillBase skillData;
    private Vector3 direction;
    private float speed = 20f;
    private float maxLifeTime = 5f;

    public GameObject explosionVFX;

    private bool hasHit = false; // Ngăn nổ nhiều lần

    public void Init(SkillBase data, Vector3 dir)
    {
        skillData = data;
        direction = dir;
        Destroy(gameObject, maxLifeTime);
    }

    private void Update()
    {
        if (!hasHit)
        {
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        // Chỉ xử lý khi chạm Enemy
        if (other.CompareTag("Enemy"))
        {
            hasHit = true;

            // TODO: Gây sát thương nếu enemy có script như EnemyHealth
            // other.GetComponent<EnemyHealth>()?.TakeDamage(skillData.power);

            // Hiệu ứng nổ
            if (explosionVFX != null)
            {
                GameObject VFX = Instantiate(explosionVFX, transform.position, Quaternion.identity);
                Destroy(VFX, 2f); // Xoá hiệu ứng sau 2 giây
            }

            Destroy(gameObject);
        }
    }
}
