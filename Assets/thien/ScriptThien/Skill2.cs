using UnityEngine;
using System.Collections;

public class Skill2 : MonoBehaviour
{
    [Header("Attack Animations")]
    [SerializeField] private string skill2Animation = "skill201";
    [SerializeField] private string skill2TeleportAnimation = "skill202";

    [Header("Skill Settings")]
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float teleportDistance = 7f;
    [SerializeField] private float effectSpeed = 10f;
    [SerializeField] private float postTeleportEffectDelay = 0f;

    [Header("Effect Settings")]
    public ParticleSystem skill201;
    public ParticleSystem skill202; // Hiệu ứng teleport sau khi đến đích
    public ParticleSystem attack2VFX;

    private Animator animator;
    private Transform currentTarget;
    private bool isAttacking = false;
    private bool canTeleport = false;
    private Vector3 skillHitPosition;
    private ParticleSystem currentSkill201Effect; // Lưu trữ hiệu ứng skill201 đang chạy
    private ParticleSystem currentSkill202Effect; // Lưu trữ hiệu ứng skill202 đang chạy

    private void Awake()
    {
        animator = GetComponent<Animator>();

        // TẮT TẤT CẢ HIỆU ỨNG KHI BẮT ĐẦU GAME
        if (skill201 != null) skill201.Stop();
        if (skill202 != null) skill202.Stop();
        if (attack2VFX != null) attack2VFX.Stop();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && !isAttacking)
        {
            FindNearestEnemy();

            if (currentTarget != null)
            {
                float distance = Vector3.Distance(transform.position, currentTarget.position);

                if (distance <= 15f)
                {
                    StartSkill2();
                }
                else
                {
                    Debug.Log($"❌ Quá xa - {distance:F2}m (Cần <15m)");
                    currentTarget = null;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.F) && canTeleport && currentTarget != null)
        {
            TeleportForwardFromHit();
        }
    }

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
                currentTarget = enemy.transform;
            }
        }
    }

    private void StartSkill2()
    {
        if (!isAttacking && currentTarget != null)
        {
            // Dừng tất cả hiệu ứng cũ trước khi bắt đầu skill mới
            StopAllEffects();

            isAttacking = true;
            canTeleport = false;
            animator.Play(skill2Animation);
            FaceEnemy();
            StartCoroutine(ActivateSkillEffect());
        }
    }

    private IEnumerator ActivateSkillEffect()
    {
        yield return new WaitForSeconds(1f);

        if (currentTarget != null && skill201 != null)
        {
            // Dừng hiệu ứng cũ nếu có
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
    }

    private void TeleportForwardFromHit()
    {
        if (currentTarget == null) return;

        // Dừng tất cả hiệu ứng cũ trước khi teleport
        StopAllEffects();

        // Tính toán vị trí teleport
        Vector3 directionToHit = (skillHitPosition - transform.position).normalized;
        Vector3 teleportPosition = skillHitPosition + directionToHit * teleportDistance;
        teleportPosition.y = transform.position.y;

        // Thực hiện teleport
        transform.position = teleportPosition;
        FaceEnemy();

        // Play animation teleport
        if (!string.IsNullOrEmpty(skill2TeleportAnimation))
        {
            animator.Play(skill2TeleportAnimation);
        }

        // Bắt đầu coroutine để kích hoạt hiệu ứng sau khi animation chạy được 0.1 giây
        StartCoroutine(PlayTeleportEffectAfterDelay());

        // Reset trạng thái
        canTeleport = false;
        currentTarget = null;
        isAttacking = false;
    }

    private IEnumerator PlayTeleportEffectAfterDelay()
    {
        // Đợi cho đến khi animation "skill202" chạy được 0.1 giây (10%)
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.1f)
        {
            yield return null;
        }

        // Dừng hiệu ứng cũ nếu có
        if (currentSkill202Effect != null)
        {
            currentSkill202Effect.Stop();
            Destroy(currentSkill202Effect.gameObject);
        }

        // Kích hoạt hiệu ứng teleport (skill202)
        if (skill202 != null)
        {
            currentSkill202Effect = Instantiate(skill202, transform.position, transform.rotation);
            currentSkill202Effect.Play();
            Destroy(currentSkill202Effect.gameObject, 2f);
        }

        // Kích hoạt hiệu ứng tấn công (attack2VFX)
        if (attack2VFX != null)
        {
            attack2VFX.Stop(); // Dừng trước khi chạy lại
            attack2VFX.Play();
        }
    }

    private void StopAllEffects()
    {
        // Dừng và hủy skill201 nếu đang chạy
        if (currentSkill201Effect != null)
        {
            currentSkill201Effect.Stop();
            Destroy(currentSkill201Effect.gameObject);
            currentSkill201Effect = null;
        }

        // Dừng và hủy skill202 nếu đang chạy
        if (currentSkill202Effect != null)
        {
            currentSkill202Effect.Stop();
            Destroy(currentSkill202Effect.gameObject);
            currentSkill202Effect = null;
        }

        // Dừng attack2VFX nếu đang chạy
        if (attack2VFX != null)
        {
            attack2VFX.Stop();
        }
    }

    private void FaceEnemy()
    {
        if (currentTarget == null) return;

        Vector3 direction = (currentTarget.position - transform.position).normalized;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction);
    }
}