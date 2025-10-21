// NPCSpeechBubble_TMP.cs
using System.Collections;
using UnityEngine;
using TMPro;

public class NPCSpeechBubble_TMP : MonoBehaviour
{
    [Header("Components")]
    public TextMeshProUGUI tmpText; // kéo từ prefab
    public Transform targetToFace; // camera transform, để null sẽ tự tìm Camera.main

    [Header("Messages")]
    [TextArea(2, 6)]
    public string[] messages; // danh sách message

    [Header("Timing (seconds)")]
    public float showDuration = 2f;      // hiển thị mỗi lần bao lâu
    public float intervalMin = 4f;       // khoảng chờ tối thiểu giữa 2 lần
    public float intervalMax = 8f;       // khoảng chờ tối đa giữa 2 lần
    public bool showOnStart = false;     // có hiện 1 message ở Start không

    private CanvasGroup canvasGroup;

    void Awake()
    {
        if (tmpText == null) Debug.LogWarning("tmpText chưa được assign!");
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // ẩn ban đầu
        canvasGroup.alpha = 0f;
    }

    void Start()
    {
        if (messages == null || messages.Length == 0) return;

        if (showOnStart) StartCoroutine(ShowRandomLoop(true));
        else StartCoroutine(ShowRandomLoop(false));
    }

    

    private IEnumerator ShowRandomLoop(bool immediate)
    {
        if (immediate)
        {
            StartCoroutine(ShowOnce());
        }

        while (true)
        {
            float wait = Random.Range(intervalMin, intervalMax);
            yield return new WaitForSeconds(wait);
            yield return StartCoroutine(ShowOnce());
        }
    }

    private IEnumerator ShowOnce()
    {
        if (messages == null || messages.Length == 0) yield break;
        int idx = Random.Range(0, messages.Length);
        tmpText.text = messages[idx];

        // fade in
        yield return StartCoroutine(FadeCanvas(0f, 1f, 0.12f));
        // giữ text
        yield return new WaitForSeconds(showDuration);
        // fade out
        yield return StartCoroutine(FadeCanvas(1f, 0f, 0.12f));
        tmpText.text = "";
    }

    private IEnumerator FadeCanvas(float from, float to, float time)
    {
        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, t / time);
            yield return null;
        }
        canvasGroup.alpha = to;
    }
}
