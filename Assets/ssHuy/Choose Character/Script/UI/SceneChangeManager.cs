using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneChangeManager : MonoBehaviour
{
    public static SceneChangeManager Instance;

    [Header("Loading Settings")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Animator loadingAnimator;
    [SerializeField] private float openAnimTime = 1f;
    [SerializeField] private float closeAnimTime = 1f;

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
    /// Load scene bình thường (tự đóng)
    /// </summary>
    public void ChangeScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsyncRoutine(sceneName));
    }

    /// <summary>
    /// Load scene xong nhưng KHÔNG tự đóng loading — cho phép script khác điều khiển thời điểm fade out.
    /// </summary>
    public IEnumerator LoadSceneAndWaitClose(string sceneName)
    {
        if (loadingScreen != null)
            loadingScreen.SetActive(true);

        if (loadingAnimator != null)
            loadingAnimator.SetBool("IsOpen", true);

        yield return new WaitForSeconds(openAnimTime);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
            yield return null;

        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
            yield return null;

        // Dừng ở đây để bên ngoài quyết định khi nào đóng animation
        yield return null;
    }

    /// <summary>
    /// Gọi hàm này khi đã cập nhật xong dữ liệu sau load.
    /// </summary>
    public IEnumerator CloseLoadingScreen()
    {
        if (loadingAnimator != null)
            loadingAnimator.SetBool("IsOpen", false);

        yield return new WaitForSeconds(closeAnimTime);

        if (loadingScreen != null)
            loadingScreen.SetActive(false);
    }

    public IEnumerator LoadSceneAsyncRoutine(string sceneName)
    {
        yield return LoadSceneAndWaitClose(sceneName);
        yield return CloseLoadingScreen();
    }
}
