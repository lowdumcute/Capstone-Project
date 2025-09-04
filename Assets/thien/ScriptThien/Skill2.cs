using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class Skill2 : MonoBehaviour
{
    [Header("Cài đặt Animation Tấn Công")]
    [SerializeField] private string skill2Animation = "skill201";        // Animation tấn công thường
    [SerializeField] private string skill2TeleportAnimation = "skill202"; // Animation khi dịch chuyển

    [Header("Cài đặt Kỹ Năng")]
    [SerializeField] private float detectionRange = 20f;  // Tầm phát hiện kẻ địch tối đa
    [SerializeField] private float teleportDistance = 7f; // Khoảng cách dịch chuyển vượt qua mục tiêu
    [SerializeField] private float effectSpeed = 10f;     // Tốc độ bay của hiệu ứng skill
    [SerializeField] private float postTeleportEffectDelay = 0f; // Thời gian delay sau khi dịch chuyển để phát hiệu ứng

    [Header("Cooldown Settings")]
    [SerializeField] private float skillCooldown = 10f; // Thời gian hồi chiêu
    [SerializeField] private Image cooldownImage; // UI Image fill
    [SerializeField] private  TextMeshProUGUI  cooldownText; // UI Text hiển thị số giây
    private bool isCooldown = false;
    private float cooldownTimer = 0f;


    [Header("Cài đặt Hiệu Ứng")]
    public ParticleSystem skill201;   // Hiệu ứng khi tung skill chính (projectile)
    public ParticleSystem skill202;   // Hiệu ứng khi dịch chuyển
    public ParticleSystem skill203;   // Hiệu ứng phụ bổ sung (giống aura hay buff)
    public ParticleSystem attack2VFX; // Hiệu ứng bổ sung khi đánh trúng

    private Animator animator;             // Animator để điều khiển animation
    private Transform currentTarget;       // Mục tiêu hiện tại
    private bool isAttacking = false;      // Đang trong trạng thái tấn công hay không
    private bool canTeleport = false;      // Có thể dịch chuyển hay không
    private Vector3 skillHitPosition;      // Vị trí trúng mục tiêu của skill

    // Biến lưu instance hiệu ứng đang chạy (để xóa khi cần)
    private ParticleSystem currentSkill201Effect;
    private ParticleSystem currentSkill202Effect;
    private ParticleSystem currentSkill203Effect;

    private Player_Controller playerController; // Tham chiếu đến script điều khiển nhân vật

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<Player_Controller>(); // Lấy tham chiếu tới Player_Controller

        // 🔹 Tắt tất cả hiệu ứng ngay khi bắt đầu game (tránh phát ngẫu nhiên khi chưa kích hoạt skill)
        if (skill201 != null) skill201.Stop();
        if (skill202 != null) skill202.Stop();
        if (attack2VFX != null) attack2VFX.Stop();
        if (skill203 != null) skill203.Stop();
    }

    private void Update()
    {
        // Cooldown update
        if (isCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            float fill = cooldownTimer / skillCooldown;
            cooldownImage.fillAmount = fill;
            cooldownText.text = Mathf.CeilToInt(cooldownTimer).ToString();

            if (cooldownTimer <= 0f)
            {
                isCooldown = false;
                cooldownImage.fillAmount = 0f;
                cooldownText.text = "";
            }
        }
        // 🔹 Chuột phải để bắt đầu tung skill (chỉ khi nhân vật không tấn công và có thể di chuyển)
        if (Input.GetMouseButtonDown(1) && !isAttacking && !isCooldown && playerController.canMove)
        {
            FindNearestEnemy(); // Tìm kẻ địch gần nhất

            if (currentTarget != null)
            {
                float distance = Vector3.Distance(transform.position, currentTarget.position);

                if (distance <= 15f) // Chỉ cho phép tấn công nếu mục tiêu ở trong tầm 15m
                {
                    StartSkill2(); // Kích hoạt skill
                    if (skill203 != null)
                    {
                        StartCoroutine(PlaySkill203());
                        StartCoroutine(SkillCooldownHandler()); // bắt đầu theo dõi cooldown

                    }
                }
                else
                {
                    Debug.Log($"❌ Quá xa - {distance:F2}m (Cần <15m)");
                    currentTarget = null; // Reset mục tiêu
                }
            }
        }

        // 🔹 Nhấn phím F để dịch chuyển đến sau mục tiêu (nếu được phép)
        if (Input.GetKeyDown(KeyCode.F) && canTeleport && currentTarget != null)
        {
            TeleportForwardFromHit();
            StartCooldown(); // bắt đầu hồi chiêu ngay khi teleport
        }
    }
    private void StartCooldown()
    {
        isCooldown = true;
        cooldownTimer = skillCooldown;
        cooldownImage.fillAmount = 1f;
    }
    private IEnumerator SkillCooldownHandler()
    {
        yield return new WaitForSeconds(3f);
        if (canTeleport) // Nếu người chơi chưa teleport trong 3 giây
        {
            ResetSkillState();
            StartCooldown(); // Bắt đầu hồi chiêu luôn
        }
    }


    // 🔹 Tìm kẻ địch gần nhất trong tầm detectionRange
    private void FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = Mathf.Infinity;
        currentTarget = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance < closestDistance && distance <= detectionRange)
            {
                closestDistance = distance;
                currentTarget = enemy.transform; // Cập nhật mục tiêu gần nhất
            }
        }
    }

    // 🔹 Bắt đầu thi triển kỹ năng
    private void StartSkill2()
    {
        if (!isAttacking && currentTarget != null)
        {
            // Tắt di chuyển của player trong lúc thi triển skill
            playerController.SetMovementEnabled(false);

            StopAllEffects(); // Xóa toàn bộ hiệu ứng đang chạy
            isAttacking = true;
            canTeleport = false;

            animator.Play(skill2Animation); // Chạy animation skill
            FaceEnemy(); // Quay mặt về phía kẻ địch
            StartCoroutine(ActivateSkillEffect()); // Bắt đầu hiệu ứng tấn công
        }
    }

    // 🔹 Phát hiệu ứng phụ skill203 (aura/buff), tự hủy sau 3s
    private IEnumerator PlaySkill203()
    {
        if (currentSkill203Effect != null)
        {
            currentSkill203Effect.Stop();
            Destroy(currentSkill203Effect.gameObject);
            currentSkill203Effect = null;
        }

        currentSkill203Effect = Instantiate(skill203, transform.position, transform.rotation);
        currentSkill203Effect.Play();

        yield return new WaitForSeconds(3f);

        if (currentSkill203Effect != null)
        {
            currentSkill203Effect.Stop();
            Destroy(currentSkill203Effect.gameObject);
            currentSkill203Effect = null;
        }
    }

    // 🔹 Tạo hiệu ứng projectile (skill201 bay về phía mục tiêu)
    private IEnumerator ActivateSkillEffect()
    {
        yield return new WaitForSeconds(1f); // Delay để đồng bộ với animation

        if (currentTarget != null && skill201 != null)
        {
            if (currentSkill201Effect != null)
            {
                currentSkill201Effect.Stop();
                Destroy(currentSkill201Effect.gameObject);
            }

            // Bắt đầu từ vị trí phía trước player
            Vector3 startPosition = transform.position + transform.forward;
            currentSkill201Effect = Instantiate(skill201, startPosition, transform.rotation);
            currentSkill201Effect.Play();

            Vector3 direction = (currentTarget.position - startPosition).normalized;

            float distance = Vector3.Distance(startPosition, currentTarget.position);
            float duration = distance / effectSpeed;
            float time = 0;

            // Hiệu ứng bay dần tới mục tiêu
            while (time < duration && currentTarget != null)
            {
                currentSkill201Effect.transform.position = startPosition + direction * (effectSpeed * time);
                time += Time.deltaTime;
                yield return null;
            }

            if (currentTarget != null)
            {
                skillHitPosition = currentTarget.position; // Lưu lại vị trí mục tiêu trúng
            }

            Destroy(currentSkill201Effect.gameObject, 0.1f);
        }

        isAttacking = false;
        canTeleport = true; // Cho phép dịch chuyển sau khi projectile kết thúc

        // Mở khóa di chuyển sau khi animation hoàn thành
        yield return new WaitForSeconds(1f);
        if (canTeleport) // Nếu chưa dịch chuyển thì reset skill
        {
            ResetSkillState();
        }
    }

    // 🔹 Reset trạng thái của skill về mặc định
    private void ResetSkillState()
    {
        isAttacking = false;
        canTeleport = false;
        currentTarget = null;
        playerController.SetMovementEnabled(true); // Luôn mở khóa di chuyển
        Debug.Log("Skill2: Đã tự động mở khóa di chuyển");
    }

    // 🔹 Dịch chuyển đến sau mục tiêu
    private void TeleportForwardFromHit()
    {
        if (currentTarget == null) return;

        StopAllEffects(); // Xóa toàn bộ hiệu ứng đang chạy

        Vector3 directionToHit = (skillHitPosition - transform.position).normalized;
        Vector3 teleportPosition = skillHitPosition + directionToHit * teleportDistance;
        teleportPosition.y = transform.position.y; // Giữ nguyên trục Y (không bay lên/xuống)

        transform.position = teleportPosition; // Dịch chuyển nhân vật
        FaceEnemy(); // Quay mặt về phía mục tiêu

        if (!string.IsNullOrEmpty(skill2TeleportAnimation))
        {
            animator.Play(skill2TeleportAnimation); // Animation dịch chuyển
        }

        StartCoroutine(PlayTeleportEffectAfterDelay()); // Hiệu ứng sau dịch chuyển

        canTeleport = false;
        currentTarget = null;
        isAttacking = false;

        // Bật lại di chuyển sau khi dịch chuyển xong
        playerController.SetMovementEnabled(true);
    }

    // 🔹 Phát hiệu ứng dịch chuyển và VFX sau animation teleport
    private IEnumerator PlayTeleportEffectAfterDelay()
    {
        // Chờ animation chạy một chút để đồng bộ
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.1f)
        {
            yield return null;
        }

        if (currentSkill202Effect != null)
        {
            currentSkill202Effect.Stop();
            Destroy(currentSkill202Effect.gameObject);
        }

        if (skill202 != null)
        {
            currentSkill202Effect = Instantiate(skill202, transform.position, transform.rotation);
            currentSkill202Effect.Play();
            Destroy(currentSkill202Effect.gameObject, 2f);
        }

        if (attack2VFX != null)
        {
            attack2VFX.Stop();
            attack2VFX.Play();
        }
    }

    // 🔹 Hàm dừng toàn bộ hiệu ứng đang chạy (dùng trước khi phát hiệu ứng mới)
    private void StopAllEffects()
    {
        if (currentSkill201Effect != null)
        {
            currentSkill201Effect.Stop();
            Destroy(currentSkill201Effect.gameObject);
            currentSkill201Effect = null;
        }

        if (currentSkill202Effect != null)
        {
            currentSkill202Effect.Stop();
            Destroy(currentSkill202Effect.gameObject);
            currentSkill202Effect = null;
        }

        if (attack2VFX != null)
        {
            attack2VFX.Stop();
        }
    }

    // 🔹 Quay nhân vật về phía mục tiêu
    private void FaceEnemy()
    {
        if (currentTarget == null) return;

        Vector3 direction = (currentTarget.position - transform.position).normalized;
        direction.y = 0; // Không thay đổi hướng dọc trục Y
        transform.rotation = Quaternion.LookRotation(direction);
    }
}
