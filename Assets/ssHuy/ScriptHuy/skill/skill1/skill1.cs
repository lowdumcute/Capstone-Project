using UnityEngine;

public class skill1 : SkillBehaviour
{
    public float effectRadius = 3f;
    public GameObject visualEffectPrefab;
    public float pullForce = 10f;
    public LayerMask enemyLayer;
    public float effectDuration = 1.5f;

    public override void UseSkill(Transform firePoint, GameObject target)
    {
        // Xác định vị trí dưới chân địch (ground)
        Vector3 spawnPosition = GetGroundPosition(target.transform.position);

        // Spawn hiệu ứng visual
        if (visualEffectPrefab != null)
        {
            GameObject vfx = Instantiate(visualEffectPrefab, spawnPosition, Quaternion.identity);
            Destroy(vfx, 3f); // Xóa sau 3s hoặc thời gian bạn muốn
        }

        // Hút các enemy trong vùng
        Collider[] enemies = Physics.OverlapSphere(spawnPosition, effectRadius, enemyLayer);

        foreach (var enemy in enemies)
        {
            if (enemy.attachedRigidbody != null)
            {
                Vector3 dir = (spawnPosition - enemy.transform.position).normalized;
                enemy.attachedRigidbody.AddForce(dir * pullForce, ForceMode.Impulse);
            }
        }
    }

    private Vector3 GetGroundPosition(Vector3 startPos)
    {
        RaycastHit hit;
        if (Physics.Raycast(startPos + Vector3.up * 5f, Vector3.down, out hit, 10f))
        {
            return hit.point;
        }
        return startPos;
    }
}
