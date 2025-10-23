using UnityEngine;

public enum SkillType { Attack, Buff, Heal, Summon }
public enum StatusEffectType { None, Burn, Poison, Stun }

[CreateAssetMenu(menuName = "Skills/New Skill")]
public class SkillBase : ScriptableObject
{
    public string skillName;
    public SkillType skillType;
    [Header("Stats")]
    public float power; // Damage, Heal hoặc Buff power
    public StatusEffectType statusEffect;
    public float statusDuration;
    public float cooldown; // Thời gian hồi chiêu
}