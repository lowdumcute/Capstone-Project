using UnityEngine;

public abstract class SkillBehaviour : MonoBehaviour
{
    public SkillBase skillData;

    public virtual void UseSkill(Transform firePoint, GameObject target)
    {
    }
}
