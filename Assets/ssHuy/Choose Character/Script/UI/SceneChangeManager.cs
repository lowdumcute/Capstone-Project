using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneChangeManager : MonoBehaviour
{
    public static SceneChangeManager Instance;

    [Header("Loading Settings")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Animator loadingAnimator; // Animator hiệu ứng mở/đóng
    [SerializeField] private float openAnimTime = 1f;  // thời gian animation mở
    [SerializeField] private float closeAnimTime = 1f; // thời gian animation đóng

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Gọi để đổi scene, có hiệu ứng chuyển cảnh mượt mà.
    /// </summary>
    public void ChangeScene(string sceneName)
    {
        StartCoroutine(LoadSceneSmart(sceneName));
    }

    private IEnumerator LoadSceneSmart(string sceneName)
    {
        // Kích hoạt UI loading
        if (loadingScreen != null)
            loadingScreen.SetActive(true);

        // Bắt đầu animation mở (fade in)
        if (loadingAnimator != null)
            loadingAnimator.SetBool("IsOpen", true);

        // Chờ animation mở chạy xong trước khi load scene (tránh lag)
        yield return new WaitForSeconds(openAnimTime);

        // Bắt đầu load scene bất đồng bộ
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        // Đợi scene load gần xong (0.9f = Unity đã load xong nhưng chưa kích hoạt)
        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                // Khi load xong, kích hoạt scene
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }

        // Đợi 1 frame để đảm bảo scene đã kích hoạt
        yield return null;

        // Sau khi scene load xong, đóng animation (fade out)
        if (loadingAnimator != null)
            loadingAnimator.SetBool("IsOpen", false);

        // Đợi animation đóng xong trước khi ẩn loading screen
        yield return new WaitForSeconds(closeAnimTime);

        if (loadingScreen != null)
            loadingScreen.SetActive(false);
    }
}
