using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(EnemyHealth))]
public class EnemyHealthBar : MonoBehaviour
{
    public Slider fillslider; // drag the fill Image (type = Filled or use width)
    public float hideWhenFullDelay = 0f; // nếu muốn ẩn khi full
    public EnemyHealth Health;


    void Awake()
    {
        if (fillslider == null)
            Debug.LogWarning("EnemyHealthBar: fillImage chưa gán.");
    }

    public void SetMaxHealth(float max)
    {
        Health.maxHealth = max;
        SetHealth(max);
    }

    public void SetHealth(float current)
    {
        if (fillslider == null) return;
        float normalized = Mathf.Clamp01(current / Health.maxHealth);
        // nếu dùng Image.fillAmount:
        fillslider.value = normalized;
    }

    //void LateUpdate()
    //{
    //    // Billboard: quay về camera để luôn hướng với người chơi
    //    if (Camera.main != null)
    //        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    //}
}
