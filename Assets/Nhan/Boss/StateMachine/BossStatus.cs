using UnityEngine;

public class BossStatus : MonoBehaviour
{
    public float Health;
    public float MaxHealth;

    public float AttackRangeFromCLose = 4f;
    public float AttackRangeFromFar = 15f;

    public BoxCollider HitBoxAttack;

    public void takeDamage(float Damage)
    {
        Health -= Damage;
    }
    public void enabledHitBox()
    {
        HitBoxAttack.enabled = true;
    }
    public void disabledHitBox()
    {
        HitBoxAttack.enabled = false;
    }


}
