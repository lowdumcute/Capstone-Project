using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class HoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public RectTransform targetTransform; // Gán vào UI có RectTransform
    public TextMeshProUGUI text; // Gán vào TextMeshPro cần hiện
    [SerializeField] private float scaleSpeed = 5f; // Tốc độ phóng to/thu nhỏ

    private bool isHovered = false;
    private bool isLocked = false;

    private Coroutine widthCoroutine;
    private Coroutine fadeCoroutine;

    private void Start()
    {
        if (targetTransform != null)
            targetTransform.sizeDelta = new Vector2(0, targetTransform.sizeDelta.y); // Bắt đầu với width = 0

        if (text != null)
            text.alpha = 0; // Ẩn text ban đầu
    }

    private void Update()
    {
        if (isHovered || isLocked) return;

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            StartEffect(0, 0); // Reset về trạng thái ban đầu
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isLocked) return;
        isHovered = true;
        StartEffect(350, 1); // Mở rộng & hiện chữ
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isLocked) return;
        isHovered = false;
        StartEffect(0, 0); // Thu nhỏ & ẩn chữ ngay lập tức
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        isLocked = !isLocked; // Toggle trạng thái lock
    }

    private void StartEffect(float targetWidth, float targetAlpha)
    {
        if (widthCoroutine != null) StopCoroutine(widthCoroutine);
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        widthCoroutine = StartCoroutine(ChangeWidth(targetWidth));
        fadeCoroutine = StartCoroutine(FadeText(targetAlpha));
    }

    private IEnumerator ChangeWidth(float targetWidth)
    {
        while (Mathf.Abs(targetTransform.sizeDelta.x - targetWidth) > 0.1f)
        {
            float newWidth = Mathf.Lerp(targetTransform.sizeDelta.x, targetWidth, Time.deltaTime * scaleSpeed * 5);
            targetTransform.sizeDelta = new Vector2(newWidth, targetTransform.sizeDelta.y);
            yield return null;
        }
        targetTransform.sizeDelta = new Vector2(targetWidth, targetTransform.sizeDelta.y);
    }

    private IEnumerator FadeText(float targetAlpha)
    {
        while (Mathf.Abs(text.alpha - targetAlpha) > 0.01f)
        {
            text.alpha = Mathf.Lerp(text.alpha, targetAlpha, Time.deltaTime * scaleSpeed * 5);
            yield return null;
        }
        text.alpha = targetAlpha;
    }
}
