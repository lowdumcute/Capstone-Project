using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class HoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("UI References")]
    public RectTransform targetTransform; // UI panel
    public TMP_Text targetText; // Text cần đổi màu và scale

    [Header("Effect Settings")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float textScaleSpeed = 8f;
    [SerializeField] private Color hoverColor = new Color(1f, 0.9f, 0.4f); // vàng nhẹ
    private Color originalColor;
    private Vector3 originalScale;

    private bool isHovered = false;
    private bool isLocked = false;

    private Coroutine moveCoroutine;
    private Coroutine textEffectCoroutine;

    private void Start()
    {
        if (targetTransform != null)
            targetTransform.anchoredPosition = new Vector2(610, targetTransform.anchoredPosition.y);

        if (targetText != null)
        {
            originalColor = targetText.color;
            originalScale = targetText.transform.localScale;
        }
    }

    private void Update()
    {
        if (isHovered || isLocked) return;

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            StartMove(610); // Reset về vị trí ban đầu
            StartTextEffect(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isLocked) return;
        isHovered = true;

        StartMove(0); // Di chuyển x về 0
        StartTextEffect(true); // Phóng to + đổi màu
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isLocked) return;
        isHovered = false;

        StartMove(610); // Di chuyển về x = 610
        StartTextEffect(false); // Trả về bình thường
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        isLocked = !isLocked;
    }

    private void StartMove(float targetX)
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveToX(targetX));
    }

    private IEnumerator MoveToX(float targetX)
    {
        while (Mathf.Abs(targetTransform.anchoredPosition.x - targetX) > 0.1f)
        {
            float newX = Mathf.Lerp(targetTransform.anchoredPosition.x, targetX, Time.deltaTime * moveSpeed);
            targetTransform.anchoredPosition = new Vector2(newX, targetTransform.anchoredPosition.y);
            yield return null;
        }
        targetTransform.anchoredPosition = new Vector2(targetX, targetTransform.anchoredPosition.y);
    }

    private void StartTextEffect(bool toHover)
    {
        if (textEffectCoroutine != null)
            StopCoroutine(textEffectCoroutine);
        textEffectCoroutine = StartCoroutine(ChangeTextEffect(toHover));
    }

    private IEnumerator ChangeTextEffect(bool toHover)
    {
        Vector3 targetScale = toHover ? originalScale * 1.1f : originalScale;
        Color targetColor = toHover ? hoverColor : originalColor;

        while (Vector3.Distance(targetText.transform.localScale, targetScale) > 0.01f ||
               targetText.color != targetColor)
        {
            targetText.transform.localScale = Vector3.Lerp(targetText.transform.localScale, targetScale, Time.deltaTime * textScaleSpeed);
            targetText.color = Color.Lerp(targetText.color, targetColor, Time.deltaTime * textScaleSpeed);
            yield return null;
        }

        targetText.transform.localScale = targetScale;
        targetText.color = targetColor;
    }
}
