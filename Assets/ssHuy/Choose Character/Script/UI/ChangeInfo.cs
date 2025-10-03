using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeInfo : MonoBehaviour
{
    [SerializeField] private UIStatsRadarChart uiStatsRadarChart;
    [SerializeField] private BaseStats playerStats;
    [SerializeField] private Sprite AvatarImage; // Ảnh hiển thị trên UI
    [SerializeField] private GameObject Avatar; // GameObject chứa Image cần thay đổi
    [SerializeField] private AudioClip clickSound; // Âm thanh khi click
    [SerializeField] private float fadeDuration = 0.5f; // Thời gian làm rõ ảnh
    [Header("Audio Source")]
    [SerializeField] private TMP_Text AttackText;
    [SerializeField] private TMP_Text DefenseText;
    [SerializeField] private TMP_Text SpeedText;
    [SerializeField] private TMP_Text ManaText;
    [SerializeField] private TMP_Text HealthText;
    private AudioSource audioSource;

    private void Start()
    {
        // Kiểm tra và thêm AudioSource nếu chưa có
        audioSource = Avatar.GetComponent<AudioSource>();
    }

    public void ChangeStat()
    {
        Stats stats = new Stats(playerStats.BAttack, playerStats.BDefense, playerStats.BSpeed, playerStats.BMana, playerStats.BHealth);
        uiStatsRadarChart.SetStats(stats);
        ChangeAvatar(AvatarImage);
        ChangeStatsText();
    }

    public void ChangeAvatar(Sprite newSprite)
    {
        if (Avatar != null)
        {
            Image avatarImg = Avatar.GetComponent<Image>(); // Lấy component Image từ Avatar
            if (avatarImg != null && newSprite != null)
            {
                // Thay đổi ảnh mới
                avatarImg.sprite = newSprite;
                StartCoroutine(FadeInImageAlpha(avatarImg));
                

                PlayClickSound(clickSound); // Phát âm thanh
            }
            else
            {
                Debug.LogWarning("Avatar không có component Image hoặc newSprite bị null!");
            }
        }
        else
        {
            Debug.LogWarning("Avatar GameObject bị null!");
        }
    }

    private IEnumerator FadeInImageAlpha(Image avatarImg)
    {
        // Reset alpha về 0 trước khi bắt đầu
        Color originalColor = avatarImg.color;
        avatarImg.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f); // Đặt alpha = 0

        // Tạo màu mục tiêu với alpha = 1
        Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
        float elapsedTime = 0f;

        // Từ alpha = 0 (mờ) đến alpha = 1 (rõ)
        while (elapsedTime < fadeDuration)
        {
            avatarImg.color = Color.Lerp(avatarImg.color, targetColor, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Đảm bảo màu ảnh cuối cùng là màu với alpha = 1
        avatarImg.color = targetColor;
    }


    private void PlayClickSound(AudioClip newClickSound)
    {
        if (audioSource != null && newClickSound != null)
        {
            audioSource.Stop(); // Dừng âm thanh hiện tại
            clickSound = newClickSound; // Thay đổi AudioClip
            audioSource.PlayOneShot(clickSound); // Phát âm thanh mới
        }
        else
        {
            Debug.LogWarning("Thiếu AudioSource hoặc AudioClip!");
        }
    }
    private void ChangeStatsText()
    {
        AttackText.text = ("Attack: " + playerStats.BAttack).ToString();
        DefenseText.text = ("Defense: " + playerStats.BDefense).ToString();
        SpeedText.text = ("Speed: " + playerStats.BSpeed).ToString();
        ManaText.text = ("Mana: " + playerStats.BMana).ToString();
        HealthText.text = ("Health: " + playerStats.BHealth).ToString();
    }

}
