using UnityEngine;

public class PiercingArrowSkill : SkillBehaviour
{
    public GameObject arrowPrefab; // Prefab riêng cho mũi tên

    public override void UseSkill(Transform firePoint, GameObject target)
    {
        if (arrowPrefab == null || target == null) return;

        Vector3 direction = (target.transform.position - firePoint.position).normalized;

        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.LookRotation(direction));

        PiercingArrow arrowScript = arrow.GetComponent<PiercingArrow>();
        if (arrowScript != null)
        {
            arrowScript.Init(skillData, direction);
        }
    }
}
