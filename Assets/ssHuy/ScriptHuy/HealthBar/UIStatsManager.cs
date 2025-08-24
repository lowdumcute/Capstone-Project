using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIStatsManager : MonoBehaviour
{
    public static UIStatsManager Instance;

    [Header("Health")]
    [SerializeField] private Image healthFrontFill;
    [SerializeField] private Image healthBackFill;
    [SerializeField] private TMP_Text healthText;

    [Header("Mana")]
    [SerializeField] private Image manaFrontFill;
    [SerializeField] private Image manaBackFill;
    [SerializeField] private TMP_Text manaText;

    [Header("Smooth Settings")]
    [SerializeField] private float frontSpeed = 5f;
    [SerializeField] private float backSpeed = 2f;
    [SerializeField] private float delayBeforeBack = 0.3f;

    private Coroutine healthCoroutine;
    private Coroutine manaCoroutine;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        float target = Mathf.Clamp01(currentHealth / maxHealth);
        healthText.text = $"{Mathf.RoundToInt(currentHealth)}/{Mathf.RoundToInt(maxHealth)}";

        if (healthCoroutine != null)
            StopCoroutine(healthCoroutine);

        healthCoroutine = StartCoroutine(SmoothDoubleFill(healthFrontFill, healthBackFill, target));
    }

    public void UpdateMana(float currentMana, float maxMana)
    {
        float target = Mathf.Clamp01(currentMana / maxMana);
        manaText.text = $"{Mathf.RoundToInt(currentMana)}/{Mathf.RoundToInt(maxMana)}";

        if (manaCoroutine != null)
            StopCoroutine(manaCoroutine);

        manaCoroutine = StartCoroutine(SmoothDoubleFill(manaFrontFill, manaBackFill, target));
    }

    private IEnumerator SmoothDoubleFill(Image front, Image back, float target)
    {
        float currentFront = front.fillAmount;
        float currentBack = back.fillAmount;

        // Front fill = cập nhật ngay
        float t = 0f;
        while (Mathf.Abs(front.fillAmount - target) > 0.001f)
        {
            t += Time.deltaTime * frontSpeed;
            front.fillAmount = Mathf.Lerp(currentFront, target, t);
            yield return null;
        }
        front.fillAmount = target;

        // Delay cho back fill tụt sau
        yield return new WaitForSeconds(delayBeforeBack);

        float b = 0f;
        while (Mathf.Abs(back.fillAmount - target) > 0.001f)
        {
            b += Time.deltaTime * backSpeed;
            back.fillAmount = Mathf.Lerp(currentBack, target, b);
            yield return null;
        }
        back.fillAmount = target;
    }
}
