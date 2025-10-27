using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    [Header("Cấu hình sát thương")]
    public float damageAmount = 50f;
    public string targetTag = "Player";

    private Collider weaponCollider;
    private bool canDealDamage = false;

    private void Start()
    {
        // Lấy collider của kiếm (phải có collider dạng trigger)
        weaponCollider = GetComponent<Collider>();
        if (weaponCollider == null)
        {
            Debug.LogWarning($"{name} không có Collider! Vui lòng thêm Collider và bật Is Trigger.");
        }
    }

    // 🔥 Hàm này sẽ được gọi bằng Animation Event ở frame 19
    public void DealDamageAtFrame()
    {
        canDealDamage = true;
        Debug.Log("🗡️ Frame 19 kích hoạt - chuẩn bị gây sát thương!");

        // Kiểm tra ngay lập tức Player nào đang trong vùng collider (nếu có)
        Collider[] hits = Physics.OverlapBox(
            weaponCollider.bounds.center,
            weaponCollider.bounds.extents,
            weaponCollider.transform.rotation
        );

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag(targetTag))
            {
                PlayerStats playerStats = hit.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    playerStats.TakeDamage(damageAmount);
                    Debug.Log($"⚔ Gây {damageAmount} damage cho {hit.name}");
                }
                else
                {
                    Debug.LogWarning("Không tìm thấy PlayerStats trên đối tượng bị đánh!");
                }
            }
        }

        canDealDamage = false;
    }
}
