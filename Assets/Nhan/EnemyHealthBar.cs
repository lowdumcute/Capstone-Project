using UnityEngine;
using UnityEngine.UI;


public class EnemyHealthBar : MonoBehaviour
{
    [Header("Sliders (UI)")]
    public Slider fillSlider;     // thanh máu chính (màu đỏ)
    public Slider easeSlider;     // thanh easing (màu vàng / da cam)

    [Header("Health")]
    public EnemyHealth enemyStatus;
   

    [Header("Ease Settings")]
    [Tooltip("Tốc độ mà easeSlider trượt về giá trị fill (units per second).")]
    public float easeSpeed = 10f; // tốc độ giảm của thanh vàng (units per second)
    [Tooltip("Độ chậm khi bắt đầu ease (thời gian delay trước khi thanh vàng bắt đầu giảm).")]
    public float easeDelay = 0.1f; // delay nhỏ trước khi bắt đầu trượt



    private void Start()
    {

        // init sliders
        if (fillSlider != null) fillSlider.value = enemyStatus.currentHealth / enemyStatus.maxHealth;
        if (easeSlider != null) easeSlider.value = enemyStatus.currentHealth / enemyStatus.maxHealth;
    }
    void LateUpdate()
    {
        easeSlider.value = Mathf.Lerp(easeSlider.value, fillSlider.value, 0.015f);
    }

    /// <summary>
    /// Gọi để gây sát thương cho enemy
    /// </summary>
    /// <param name="amount">Số máu mất (dương)</param>
    public void UpdateHealthBar(float amount)
    {
        if (amount <= 0) return;


        fillSlider.value = enemyStatus.currentHealth / enemyStatus.maxHealth;
        
            


        // start ease delay (thanh vàng sẽ bắt đầu trượt sau easeDelay giây)
        //easeTimer = 0f;

        // tùy: bạn có thể play VFX / sound ở đây
        // PlayHitFeedback();
    }

    /// <summary>
    /// Tùy: đặt lại thanh đầy (ví dụ khi respawn)
    /// </summary>
    public void ResetHealth()
    {
        enemyStatus.maxHealth = enemyStatus.maxHealth;
        if (fillSlider != null) fillSlider.value = 1f;
        if (easeSlider != null) easeSlider.value = 1f;

    }
}
