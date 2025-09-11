using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script xử lý Skill1 (phím E) cho player.
/// Chức năng:
/// - Khóa control player khi dùng skill
/// - Chạy animation, particle effects, đổi material kiếm, spawn hiệu ứng chém
/// - Quản lý UI cooldown (Image fill + TextMeshPro)
/// NOTE: Mình chỉ thêm comment chi tiết, không thay đổi logic.
/// </summary>
public class Skill1 : MonoBehaviour
{
    // ------------------------------
    // Animation & timing settings
    // ------------------------------
    [Header("Animation Settings")]
    public Animator playerAnimator;                      // Animator của nhân vật, dùng để phát animation skill
    public string skillAnimationName = "skill1";         // Tên animation clip trong AnimatorController
    [Tooltip("Tốc độ animation sẽ giảm xuống 0.2 sau khi hết thời gian đóng băng")]
    public float slowedDownSpeed = 0.01f;                // Tốc độ animation tạm thời khi hiệu ứng "chậm" diễn ra

    // ------------------------------
    // Particle / VFX
    // ------------------------------
    [Header("Effect Settings")]
    public ParticleSystem mathuatVFX1;                   // Particle "ma thuật" bắt đầu khi kích hoạt skill
    public ParticleSystem quanlimathuat2;                // Particle hỗ trợ, sẽ bị tắt sau 1 khoảng
    public ParticleSystem tuNangLuong;                   // Particle đại diện "tự năng lượng" (năng lượng đang tích)
    public ParticleSystem chemkill1;                     // Particle cho hiệu ứng chém (sẽ được Instantiate)
    public GameObject chay;                              // GameObject nhỏ (ví dụ lửa) gắn lên kiếm khi skill active
    [Tooltip("Thời gian playback của từ năng lượng (giây)")]
    public float tuNangLuongDuration = 9f;               // (khai báo nhưng hiện không dùng trong logic) thời lượng giả định của tuNangLuong
    public float delayBeforeAnimation = 0.8f;            // Delay trước khi phát animation (sau khi trigger skill)

    // ------------------------------
    // Sword / Weapon material
    // ------------------------------
    [Header("Sword Material Settings")]
    public Material sword12Material;                     // Material mặc định của kiếm
    public Material sword13Material;                     // Material đặc biệt khi skill active
    public Renderer swordRenderer;                       // Renderer của kiếm để gán material

    // ------------------------------
    // Player control
    // ------------------------------
    [Header("Player Control")]
    public MonoBehaviour playerController;               // Script điều khiển player (bị disable khi skill dùng)
    public KeyCode[] movementKeys = { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D }; // Các phím di chuyển cần khóa (kiểm tra trong Update)

    // ------------------------------
    // UI cooldown
    // ------------------------------
    [Header("UI Cooldown Settings")]
    public Image cooldownImage;                          // Image UI dùng fillAmount để hiển thị cooldown vòng tròn
    public float cooldownDuration = 15f;                 // Thời gian hồi chiêu (giây)
    public TextMeshProUGUI cooldownText;                 // Text hiển thị số giây còn lại

    // ------------------------------
    // Trạng thái nội bộ
    // ------------------------------
    private bool isSkillReady = true;                    // Flag: skill có thể dùng hay đang cooldown
    private float originalAnimationSpeed;                // Lưu speed animation ban đầu để restore
    private bool isTuNangLuongActive = false;            // Flag: tuNangLuong đang phát hay không
    private bool isMovementLocked = false;               // Flag: khoá input di chuyển bằng kiểm tra bàn phím (không được gán trong code hiện tại)

    // ------------------------------
    // Unity methods
    // ------------------------------
    void Start()
    {
        // ẩn object chay (lửa trên kiếm) mặc định
        chay.SetActive(false);

        // ensure tất cả particle system ở trạng thái stop (không phát)
        InitializeParticleSystems();

        // lưu lại tốc độ animation ban đầu (để khôi phục sau)
        if (playerAnimator != null)
        {
            originalAnimationSpeed = playerAnimator.speed;
        }

        // khởi tạo UI cooldown: fillAmount = 0 (không còn cooldown)
        if (cooldownImage != null)
        {
            cooldownImage.fillAmount = 0f; // ban đầu skill sẵn sàng
        }

        // hide text cooldown ban đầu
        if (cooldownText != null)
            cooldownText.text = "";
    }

    /// <summary>
    /// Dừng tất cả particle (dùng ở Start và ResetSkill để reset trạng thái VFX)
    /// </summary>
    void InitializeParticleSystems()
    {
        if (mathuatVFX1 != null) mathuatVFX1.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        if (quanlimathuat2 != null) quanlimathuat2.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        if (tuNangLuong != null) tuNangLuong.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        if (chemkill1 != null) chemkill1.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    void Update()
    {
        // Khi người chơi nhấn E, skill sẵn sàng, và tiến trình UseSkill1() của PlayerStatsManager trả true (kiểm tra tiêu hao năng lượng/skill point)
        // Ghi chú: PlayerStatsManager.Instance.UseSkill1() phải trả về true nếu đủ điều kiện dùng skill (đã cài ở script khác).
        if (Input.GetKeyDown(KeyCode.E) && isSkillReady && PlayerStatsManager.Instance.UseSkill1())
        {
            StartSkillSequence();              // bắt đầu chuỗi skill
            StartCoroutine(CooldownRoutine()); // bắt đầu cooldown UI (và set isSkillReady = true khi xong)
        }

        // Nếu đang ở trạng thái khóa di chuyển theo biến isMovementLocked,
        // ta chặn việc xử lý các phím di chuyển (WASD) — tuy nhiên hiện code chỉ return ngay khi phát hiện phím,
        // điều này có tác dụng là "không cho phép xử lý phím" trong khung Update hiện tại.
        // Lưu ý: biến isMovementLocked hiện không được set = true ở đâu trong script này.
        if (isMovementLocked)
        {
            foreach (KeyCode key in movementKeys)
            {
                if (Input.GetKeyDown(key))
                {
                    // Ngăn không cho xử lý phím di chuyển (ở đây chỉ return khỏi Update).
                    // Nếu bạn muốn chặn hoàn toàn mọi input di chuyển, nên set playerController.enabled = false khi khóa.
                    return;
                }
            }
        }
    }

    // ------------------------------
    // Skill sequence
    // ------------------------------

    /// <summary>
    /// Bắt đầu chuỗi skill: disable playerController, play particle, set trạng thái isSkillReady=false
    /// </summary>
    void StartSkillSequence()
    {
        // Vô hiệu hóa script điều khiển player => ngăn người chơi di chuyển (nếu playerController đại diện cho movement)
        if (playerController != null)
        {
            playerController.enabled = false; // Vô hiệu hóa Player_Controller khi bắt đầu tấn công
        }

        isSkillReady = false; // Đánh dấu skill đang được dùng / chưa vào cooldown xong

        // Play hiệu ứng ma thuật (nếu có), nếu chưa gán thì log warning
        if (mathuatVFX1 != null) mathuatVFX1.Play();
        else Debug.LogWarning("Particle System not assigned for Skill1");

        // Sau một delay ngắn, phát animation của skill (dùng Invoke để trì hoãn)
        Invoke("PlaySkillAnimation", delayBeforeAnimation);
    }

    /// <summary>
    /// Phát animation skill, play thêm các particle khác và schedule các hàm tắt/bật hiệu ứng theo thời gian.
    /// </summary>
    void PlaySkillAnimation()
    {
        // Phát animation trực tiếp bằng tên state (không sử dụng trigger).
        // Lưu ý: đảm bảo trong Animator có state/clip tên skillAnimationName.
        playerAnimator.Play(skillAnimationName, 0, 0f);

        // Play particle hỗ trợ
        if (quanlimathuat2 != null) quanlimathuat2.Play();

        // Tắt particle quanlimathuat2 sau 3.1s
        Invoke("ReduceParticleEffect", 3.1f);

        // Bật tuNangLuong (năng lượng) sau 1.2s
        Invoke("ActivateTuNangLuong", 1.2f);

        // Spawn hiệu ứng chém sau 3.6s
        Invoke("SpawnChemkillEffect", 3.6f); // gọi chem sau 3.6s

        // Bảo đảm Animator đã gán, nếu không thì reset skill ngay để tránh lỗi null reference
        if (playerAnimator == null)
        {
            Debug.LogWarning("Animator not assigned for Skill1");
            ResetSkill();
            return;
        }
    }

    /// <summary>
    /// Dừng particle quanlimathuat2 — gọi qua Invoke (tạm thời giảm 1 phần hiệu ứng).
    /// </summary>
    void ReduceParticleEffect()
    {
        if (quanlimathuat2 != null) quanlimathuat2.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    /// <summary>
    /// Kích hoạt "tuNangLuong" (particle), set flag và schedule việc bật kiếm (material + effect) sau ~1s.
    /// </summary>
    void ActivateTuNangLuong()
    {
        if (tuNangLuong != null)
        {
            tuNangLuong.Play();
            isTuNangLuongActive = true;

            // Sau 0.99s đổi material kiếm và hiển thị lửa
            Invoke("ActivateSwordEffects", 0.99f);
        }

        // Tiếp tục điều khiển animation (giảm speed tạm thời, tắt tuNangLuong sau 2.7s nếu active, và schedule ResetSkill dựa trên độ dài animation)
        ContinueAnimation();
    }

    /// <summary>
    /// Đổi material của kiếm sang trạng thái skill và bật GameObject "chay" (lửa).
    /// </summary>
    void ActivateSwordEffects()
    {
        if (swordRenderer != null && sword13Material != null)
        {
            swordRenderer.material = sword13Material;
            chay.SetActive(true);
        }
    }

    /// <summary>
    /// Phần xử lý tiếp tục animation:
    /// - giảm tốc animation (tạo cảm giác slow-motion)
    /// - schedule RestoreAnimationSpeed sau 1.9s
    /// - nếu tuNangLuong đang active thì schedule StopTuNangLuong sau 2.7s
    /// - schedule ResetSkill theo độ dài animation (GetAnimationLength)
    /// </summary>
    void ContinueAnimation()
    {
        if (playerAnimator != null)
        {
            playerAnimator.speed = slowedDownSpeed;      // giảm tốc độ animation để "hiệu ứng"
            Invoke("RestoreAnimationSpeed", 1.9f);       // khôi phục sau 1.9s
        }

        if (isTuNangLuongActive)
        {
            // Tắt tuNangLuong sau 2.7s (nếu đang chạy)
            Invoke("StopTuNangLuong", 2.7f);
        }

        // Khi animation kết thúc thì ResetSkill. Lưu ý: GetAnimationLength lấy bằng tên clip trong runtimeAnimatorController
        Invoke("ResetSkill", GetAnimationLength());
    }

    /// <summary>
    /// Dừng tuNangLuong particle
    /// </summary>
    void StopTuNangLuong()
    {
        if (tuNangLuong != null) tuNangLuong.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        isTuNangLuongActive = false;
    }

    /// <summary>
    /// Khôi phục speed animation về giá trị ban đầu (được lưu ở Start)
    /// </summary>
    void RestoreAnimationSpeed()
    {
        if (playerAnimator != null) playerAnimator.speed = originalAnimationSpeed;
    }

    /// <summary>
    /// Lấy độ dài (length) của animation clip theo tên skillAnimationName.
    /// Nếu không tìm thấy clip tương ứng, trả về 1f (default fallback).
    /// Ghi chú: Tên clip phải trùng chính xác với skillAnimationName.
    /// </summary>
    float GetAnimationLength()
    {
        if (playerAnimator != null)
        {
            AnimationClip[] clips = playerAnimator.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                if (clip.name == skillAnimationName)
                {
                    return clip.length;
                }
            }
        }
        // Nếu không tìm thấy clip, trả về giá trị mặc định (tránh Invoke với thời gian 0)
        return 1f;
    }

    /// <summary>
    /// Reset trạng thái skill sau khi animation kết thúc:
    /// - dừng tất cả particle
    /// - tắt hiệu ứng chay trên kiếm
    /// - đổi material về ban đầu
    /// - bật lại playerController
    /// Lưu ý: isSkillReady KHÔNG được set true ở đây vì cooldownCoroutine sẽ set khi hoàn tất.
    /// </summary>
    void ResetSkill()
    {
        // Reset tất cả particle
        InitializeParticleSystems();

        // Tắt hiệu ứng lửa trên kiếm (nếu có)
        chay.SetActive(false);

        // Đổi material kiếm về trạng thái ban đầu
        if (swordRenderer != null && sword12Material != null)
        {
            swordRenderer.material = sword12Material;
        }

        // Restore animation speed phòng trường hợp restore không được gọi
        if (playerAnimator != null) playerAnimator.speed = originalAnimationSpeed;

        // Bật lại controller player để người chơi có thể di chuyển
        if (playerController != null)
        {
            playerController.enabled = true; // Kích hoạt lại Player_Controller khi kết thúc tấn công
        }

        // isSkillReady = true;  <- BỊ COMMENT:Ở đây không set true vì bạn muốn cooldown xử lý điều đó.
        // Nếu muốn skill có thể dùng ngay khi ResetSkill, hãy bật dòng trên.
        isTuNangLuongActive = false;
    }

    /// <summary>
    /// Spawn (instantiate) hiệu ứng chém trước mặt player.
    /// - Tạo instance particle tại vị trí transform.position + transform.forward * 2f
    /// - Play effect, Destroy sau duration của effect
    /// Lưu ý: effectInstance.main.duration có thể là thời lượng emit, nhưng một số particle có looping = true => duration có thể 0.
    /// </summary>
    void SpawnChemkillEffect()
    {
        if (chemkill1 != null)
        {
            // Tính toán vị trí trước mặt player (cách 2 đơn vị)
            Vector3 spawnPosition = transform.position + transform.forward * 2f;

            // Tạo instance của particle system (để không phá vỡ prefab gốc)
            ParticleSystem effectInstance = Instantiate(chemkill1, spawnPosition, transform.rotation);

            // Kích hoạt và chạy hiệu ứng
            effectInstance.gameObject.SetActive(true);
            effectInstance.Play();

            // Hủy sau khi hoàn thành (dùng main.duration)
            // Ghi chú: nếu particle looping = true hoặc duration = 0, Destroy sẽ xóa ngay => có thể cần xử lý khác.
            Destroy(effectInstance.gameObject, effectInstance.main.duration);
        }
    }

    /// <summary>
    /// Callback (placeholder) khi animation kết thúc (có thể gọi từ Animation Event).
    /// Hiện tại để trống — bạn có thể gọi ResetSkill hoặc thêm logic khác ở đây.
    /// </summary>
    public void OnSkillAnimationEnd()
    {
        // Logic bổ sung khi kết thúc animation (nếu muốn).
        // Ví dụ: gọi ResetSkill() từ Animation Event thay vì Invoke theo GetAnimationLength.
    }

    // ------------------------------
    // Cooldown Coroutine
    // ------------------------------
    /// <summary>
    /// Coroutine quản lý UI cooldown:
    /// - cooldownImage.fillAmount từ 1 -> 0 theo thời gian
    /// - cooldownText hiển thị số giây còn lại (ceil để hiển thị số nguyên)
    /// - Khi hết thời gian: set fillAmount = 0, xóa text và set isSkillReady = true
    /// </summary>
    IEnumerator CooldownRoutine()
    {
        float elapsed = 0f;

        // Bắt đầu: hiển thị đầy vòng hồi chiêu (1)
        if (cooldownImage != null)
            cooldownImage.fillAmount = 1f;

        // Hiển thị số giây ban đầu (toString dạng "0" => không có chữ số thập phân)
        if (cooldownText != null)
            cooldownText.text = cooldownDuration.ToString("0");

        // Lặp đến khi elapsed đạt cooldownDuration
        while (elapsed < cooldownDuration)
        {
            elapsed += Time.deltaTime;

            // Cập nhật fillAmount (từ 1 xuống 0)
            if (cooldownImage != null)
                cooldownImage.fillAmount = 1f - (elapsed / cooldownDuration);

            // Cập nhật text (lấy Ceil để hiển thị số giây còn lại là số nguyên)
            if (cooldownText != null)
            {
                float remaining = Mathf.Ceil(cooldownDuration - elapsed);
                cooldownText.text = remaining.ToString("0");
            }

            yield return null; // chờ frame tiếp theo
        }

        // Khi cooldown kết thúc:
        if (cooldownImage != null)
            cooldownImage.fillAmount = 0f;

        if (cooldownText != null)
            cooldownText.text = " ";

        // Cho phép dùng skill lại
        isSkillReady = true;
    }
}
