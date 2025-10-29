using UnityEngine;
using UnityEngine.Video;

public class CutsceneManager : MonoBehaviour
{
    [Header("Video Settings")]
    public VideoPlayer cutscenePlayer;

    [Header("Scene Manager")]
    public ChooseRoleScreen chooseRoleScreen; // Tham chiếu đến script ChooseRoleScreen

    private bool hasPlayed = false;

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
        hasPlayed = true;
    }

    void OnCutsceneFinished(VideoPlayer vp)
    {
        Debug.Log("🎬 Cutscene đã chạy hết. Chuyển sang chọn vai...");
        if (chooseRoleScreen != null)
        {
            chooseRoleScreen.ChooseRole();
        }
        else
        {
            Debug.LogWarning("⚠️ Chưa gán ChooseRoleScreen trong CutsceneManager!");
        }
    }
}
