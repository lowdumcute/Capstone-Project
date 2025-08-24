using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UISkillManager : MonoBehaviour
{
    public static UISkillManager Instance { get; private set; }

    [Header("Cooldown UI")]
    [SerializeField] private Image mainSkillImage;
    [SerializeField] private TMP_Text mainSkillText;
    [SerializeField] private Image ultimateSkillImage;
    [SerializeField] private TMP_Text ultimateSkillText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    
    public void UpdateCooldownUI(float mainCooldown, float mainMaxCooldown, float ultCooldown, float ultMaxCooldown)
    {
        mainSkillImage.fillAmount = mainCooldown / mainMaxCooldown;
        if (mainCooldown <= 0)
            mainSkillText.text = "";
        else
        {
            mainSkillText.text = Mathf.CeilToInt(mainCooldown).ToString();
        }
        
        ultimateSkillImage.fillAmount = ultCooldown / ultMaxCooldown;
        if (ultCooldown <= 0)
            ultimateSkillText.text = "";
        else
        {
            ultimateSkillText.text = Mathf.CeilToInt(ultCooldown).ToString();
        }
    }
}
