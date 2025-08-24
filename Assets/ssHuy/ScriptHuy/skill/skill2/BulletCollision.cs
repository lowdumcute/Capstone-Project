using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    public GameObject vfxPrefab;
    public float offsetDistance = 0.3f; // Khoảng cách lệch khỏi tâm đạn

    private void OnTriggerEnter(Collider other)
    {
        // Chỉ xử lý nếu va chạm với enemy hoặc một số tag cụ thể (tuỳ chọn)
        if (!other.CompareTag("Enemy")) return;

        Vector3 hitDirection = (other.transform.position - transform.position).normalized;
        Vector3 spawnPosition = transform.position + hitDirection * offsetDistance;

        if (vfxPrefab != null)
        {
            Instantiate(vfxPrefab, spawnPosition, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
