using UnityEngine;

public class EnableHitBox : MonoBehaviour
{
    public BoxCollider Attack1;
    public BoxCollider Attack2;
    public BoxCollider Attack3;
    public BoxCollider SkillE;
    public BoxCollider SkillR;
    private void Start()
    {
        Attack1.enabled = false;
        Attack2.enabled = false;
        Attack3.enabled = false;
        SkillE.enabled = false;
        SkillR.enabled = false;
    }
    public void enableAttack1()
    {
        Attack1.enabled = true;
    }
    public void disableAttack1()
    {
        Attack1.enabled = false;
    }
    public void enableAttack2()
    {
        Attack2.enabled = true;
    }
    public void disableAttack2()
    {
        Attack2.enabled = false;
    }
    public void enableAttack3()
    {
        Attack3.enabled = true;
    }
    public void disableAttack3()
    {
        Attack3.enabled = false;
    }
    public void enableSkillE()
    {
        SkillE.enabled = true;
    }
    public void disableSkillE()
    {
        SkillE.enabled = false;
    }
}
