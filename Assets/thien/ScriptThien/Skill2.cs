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
    [Header("Teleport Timer UI")]
    [SerializeField] private Image teleportTimerImage; // 🔹 Image hiển thị thời gian nhấn F
    private float teleportAvailableTime = 5f; // 🔹 Thời gian 5 giây
    private float teleportTimer = 0f;

    [Header("Cài đặt Hiệu Ứng")]
    public ParticleSystem skill201;   // Hiệu ứng khi tung skill chính (projectile)
    public ParticleSystem skill202;   // Hiệu ứng khi dịch chuyển
    public ParticleSystem skill203;   // Hiệu ứng phụ bổ sung (giống aura hay buff)
   

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
    private float costMana = 40f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<Player_Controller>(); // Lấy tham chiếu tới Player_Controller

        // 🔹 Tắt tất cả hiệu ứng ngay khi bắt đầu game (tránh phát ngẫu nhiên khi chưa kích hoạt skill)
        if (skill201 != null) skill201.Stop();
        if (skill202 != null) skill202.Stop();

        if (skill203 != null) skill203.Stop();
        if (teleportTimerImage != null)
            teleportTimerImage.gameObject.SetActive(false); // 🔹 Ẩn khi game bắt đầu
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

        // Teleport timer update
        if (canTeleport && teleportTimerImage != null && teleportTimerImage.gameObject.activeSelf)
        {
            teleportTimer -= Time.deltaTime;
            teleportTimerImage.fillAmount = teleportTimer / teleportAvailableTime;

            if (teleportTimer <= 0f)
            {
                // Hết 5 giây mà chưa nhấn F
                teleportTimerImage.gameObject.SetActive(false);
                ResetSkillState();
                StartCooldown();
            }
        }

        // Chuột phải để bắt đầu tung skill
        if (Input.GetMouseButtonDown(1) && !isAttacking && !isCooldown && playerController.canMove && PlayerStats.Instance.currentMana >= costMana)
        {
            PlayerStats.Instance.UseMana(costMana);

            FindNearestEnemy();

            if (currentTarget != null)
            {
                float distance = Vector3.Distance(transform.position, currentTarget.position);

                if (distance <= 15f)
                {
                    StartSkill2(); // chỉ gọi StartSkill2, không gọi PlaySkill203 ở đây nữa
                }
                else
                {
                    Debug.Log($"❌ Quá xa - {distance:F2}m (Cần <15m)");
                    currentTarget = null;
                }
            }
        }
        // Nhấn phím F để dịch chuyển
        if (Input.GetKeyDown(KeyCode.F) && canTeleport && currentTarget != null)
        {
            TeleportForwardFromHit();
            StartCooldown();

            // 🔹 Tắt UI khi đã dịch chuyển
            if (teleportTimerImage != null)
                teleportTimerImage.gameObject.SetActive(false);
        }
    }
    private void StartCooldown()
    {
        isCooldown = true;
        cooldownTimer = skillCooldown;
        cooldownImage.fillAmount = 1f;
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
       
        if (!isAttacking && currentTarget != null )
        {
            PlayerStats.Instance.UseMana(40);////////////
            playerController.SetMovementEnabled(false);
            StopAllEffects();

            isAttacking = true;
            canTeleport = false;

            animator.Play(skill2Animation);
            FaceEnemy();
            StartCoroutine(ActivateSkillEffect());
            if (!AudioManager.Instance.sfxSource.isPlaying) // tránh chồng tiếng
                AudioManager.Instance.RCmot();

            // 🔹 Skill 203 chỉ chạy khi mana đủ
            if (skill203 != null)
                StartCoroutine(PlaySkill203());
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
        yield return new WaitForSeconds(1f);

        if (currentTarget != null && skill201 != null)
        {
            if (currentSkill201Effect != null)
            {
                currentSkill201Effect.Stop();
                Destroy(currentSkill201Effect.gameObject);
            }

            Vector3 startPosition = transform.position + transform.forward;
            currentSkill201Effect = Instantiate(skill201, startPosition, transform.rotation);
            currentSkill201Effect.Play();
           

            Vector3 direction = (currentTarget.position - startPosition).normalized;
            float distance = Vector3.Distance(startPosition, currentTarget.position);
            float duration = distance / effectSpeed;
            float time = 0;

            while (time < duration && currentTarget != null)
            {
                currentSkill201Effect.transform.position = startPosition + direction * (effectSpeed * time);
                time += Time.deltaTime;
                yield return null;
            }

            if (currentTarget != null)
            {
                skillHitPosition = currentTarget.position;
            }

            Destroy(currentSkill201Effect.gameObject, 0.1f);
        }

        isAttacking = false;
        canTeleport = true;

        // 🔹 Bật UI timer cho phép nhấn F
        if (teleportTimerImage != null)
        {
            teleportTimer = teleportAvailableTime;
            teleportTimerImage.fillAmount = 1f;
            teleportTimerImage.gameObject.SetActive(true);
        }
    }
    private void ResetSkillState()
    {
        isAttacking = false;
        canTeleport = false;
        currentTarget = null;
        playerController.SetMovementEnabled(true);

        // 🔹 Tắt UI khi reset skill
        if (teleportTimerImage != null)
            teleportTimerImage.gameObject.SetActive(false);

        Debug.Log("Skill2: Reset skill state");
    }
    // 🔹 Dịch chuyển đến sau mục tiêu
    private void TeleportForwardFromHit()
    {
        if (currentTarget == null) return;

        StopAllEffects();

        Vector3 directionToHit = (skillHitPosition - transform.position).normalized;
        Vector3 teleportPosition = skillHitPosition + directionToHit * teleportDistance;
        teleportPosition.y = transform.position.y;

        transform.position = teleportPosition;
        FaceEnemy();

        if (!string.IsNullOrEmpty(skill2TeleportAnimation))
            animator.Play(skill2TeleportAnimation);

        StartCoroutine(PlayTeleportEffectAfterDelay());
       
            AudioManager.Instance.RCHai();

        canTeleport = false;
        currentTarget = null;
        isAttacking = false;
        playerController.SetMovementEnabled(true);
    }

    // 🔹 Phát hiệu ứng dịch chuyển và VFX sau animation teleport
    private IEnumerator PlayTeleportEffectAfterDelay()
    {
        // Chờ animation bắt đầu một chút để đồng bộ
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
