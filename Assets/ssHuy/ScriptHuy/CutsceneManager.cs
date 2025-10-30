using UnityEngine;
using UnityEngine.Video;

public class CutsceneManager : MonoBehaviour
{
    [Header("Video Settings")]
    public VideoPlayer cutscenePlayer;

    [Header("Scene Manager")]
    public ChooseRoleScreen chooseRoleScreen; // Tham chiếu đến script ChooseRoleScreen

    private bool isSkipped = false;

    void Start()
    {
        if (cutscenePlayer == null)
        {
            cutscenePlayer = GetComponent<VideoPlayer>();
        }

        if (cutscenePlayer == null)
        {
            Debug.LogError("⚠️ Không tìm thấy VideoPlayer cho cutscene!");
            return;
        }

        // Đăng ký sự kiện khi video phát xong
        cutscenePlayer.loopPointReached += OnCutsceneFinished;

        // Bắt đầu phát cutscene
        cutscenePlayer.Play();
    }

    void Update()
    {
        // ✅ Nếu nhấn Enter (Return) thì skip cutscene
        if (Input.GetKeyDown(KeyCode.Return) && !isSkipped)
        {
            SkipCutscene();
        }
    }

    void SkipCutscene()
    {
        isSkipped = true;
        cutscenePlayer.Stop(); // Dừng video ngay
        Debug.Log("⏭ Cutscene bị skip bằng phím Enter.");
        OnCutsceneFinished(cutscenePlayer); // Gọi hàm chuyển cảnh luôn
    }

    void OnCutsceneFinished(VideoPlayer vp)
    {
        if (isSkipped)
            Debug.Log("🎬 Cutscene skip. Chuyển sang scene theo vai đã chọn...");
        else
            Debug.Log("🎬 Cutscene đã chạy hết. Chuyển sang scene theo vai đã chọn...");

        if (chooseRoleScreen != null)
        {
            // ✅ Gọi CheckRole() để load đúng scene tương ứng với vai
            chooseRoleScreen.CheckRole();
        }
        else
        {
            Debug.LogWarning("⚠️ Chưa gán ChooseRoleScreen trong CutsceneManager!");
        }
    }
}
