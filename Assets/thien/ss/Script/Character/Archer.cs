using System.Collections;
using UnityEngine;

public class Archer : CharacterControllerInput
{
    [Header("Bullet")]
    public GameObject arrowPrefab;
    public Transform firePoint;
    public GameObject VFX;

    [Header("Camera")]
    public Transform cameraAimPoint;
    public Transform originalCameraParent;
    [Header("Aim")]
    [SerializeField]private GameObject AimObj;
    public Transform aimTarget; // target để xoay bằng chuột
    [SerializeField] GameObject Dot;
    private bool isAiming = false;
    public bool IsAiming => isAiming;

    private Vector3 originalAimTargetLocalPosition;
    private float aimYaw = 0f;
    private float aimPitch = 0f;
    [SerializeField]private float sensitivity = 0.5f;
    [Header("Skill")]
    [SerializeField] private SkillBehaviour secondaryMove;
    [SerializeField] private float CurrentcooldownMove2;
    [SerializeField] private SkillBehaviour MoveUntil;
    [SerializeField] private float CurrentcooldownUntil;
    public GameObject windEffectPrefab; // Hiệu ứng gió khi nhảy


    protected override void Start()
    {
        Dot.SetActive(false);
        base.Start();
        AimObj.SetActive(false);
        originalAimTargetLocalPosition = aimTarget.localPosition;
        inputActions.Player.Aim.started += ctx => StartAiming();
        inputActions.Player.Aim.canceled += ctx => StopAiming();
        inputActions.Player.Attack.performed += ctx => StartCoroutine(FireArrow());
        inputActions.Player.Ulti.performed += ctx => Ultimate();
        inputActions.Player.secondaryMove.performed += ctx => Move2();
    }

    protected override void Update()
    {
        base.Update();

        if (isAiming)
        {
            HandleAimingWithRigTarget();
        }
        // Giảm dần thời gian hồi chiêu
        if (CurrentcooldownMove2 > 0)
        {
            CurrentcooldownMove2 -= Time.deltaTime;
        }
        if (CurrentcooldownUntil > 0)
        {
            CurrentcooldownUntil -= Time.deltaTime;
        }
        UISkillManager.Instance.UpdateCooldownUI(CurrentcooldownMove2, secondaryMove.skillData.cooldown, CurrentcooldownUntil, MoveUntil.skillData.cooldown);
    }

    private void StartAiming()
    {
        if (controller.isGrounded)
        {
            AimObj.SetActive(true);
            isAiming = true;

            if (animator == null)
            {
                Debug.LogError("Animator is NULL!");
                return;
            }
            Dot.SetActive(true);
            // ✅ Thêm dòng này để xoay nhân vật theo camera
            Vector3 cameraForward = mainCamera.transform.forward;
            cameraForward.y = 0f;
            if (cameraForward != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(cameraForward);

            // Disable camera script
            ThirdPersonCamera.instance.enabled = false;

            // Gắn camera vào vị trí ngắm
            mainCamera.transform.SetParent(cameraAimPoint);
            mainCamera.transform.localPosition = Vector3.zero;
            mainCamera.transform.localRotation = Quaternion.identity;

            animator.SetBool("IsAiming", true);

            Debug.Log("Aiming...");
        }
        else
        {
            Debug.LogWarning("Cannot aim while in the air!");
            return;
        }
    }

    private void StopAiming()
    {
        Dot.SetActive(false);
        AimObj.SetActive(false);
        isAiming = false;

        // Restore camera
        mainCamera.transform.SetParent(originalCameraParent);
        ThirdPersonCamera.instance.enabled = true;

        animator.SetBool("IsAiming", false);
        Debug.Log("Stopped Aiming.");

        // Đưa aimTarget về vị trí ban đầu
        aimTarget.localPosition = originalAimTargetLocalPosition;
    }

    public void Ultimate()
    {
        
        if (CurrentcooldownUntil > 0)
        {
            Debug.Log("Ultimate is on cooldown!");
            return;
        }
        StartCoroutine(UltimateSequence());
        CurrentcooldownUntil = MoveUntil.skillData.cooldown;
    }
    private IEnumerator UltimateSequence()
    {
        canMove = false;
        Transform enemy = GetNearestEnemy();
        if (enemy == null) yield break;

        gravity = 0f; // Tắt gravity
        animator.SetBool("IsAiming", true); // Bật trạng thái ngắm

        // --- Xoay nhân vật hướng về enemy (kèm 90 độ nếu cần) ---
        Vector3 direction = enemy.position - transform.position;
        direction.y = 0f;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            Quaternion extraRotation = Quaternion.Euler(0f, 90f, 0f); // Nhân vật lệch 90 độ
            transform.rotation = lookRotation * extraRotation;
        }

        // --- Bay lên ---
        GameObject VFX = Instantiate(windEffectPrefab, transform.position, Quaternion.identity);
        Destroy(VFX, 1.5f); // Xoá instance sau 3 giây
        float riseHeight = 5f;
        Vector3 originalPosition = transform.position;
        Vector3 targetPosition = originalPosition + Vector3.up * riseHeight;

        float riseTime = 0.3f;
        float elapsed = 0f;
        while (elapsed < riseTime)
        {
            transform.position = Vector3.Lerp(originalPosition, targetPosition, elapsed / riseTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;

        // --- Cập nhật camera để nhìn xuống enemy ---
        if (ThirdPersonCamera.instance != null)
        {
            ThirdPersonCamera.instance.FocusOnTarget(enemy);
            ThirdPersonCamera.instance.offset = new Vector3(0, 3, -10f);
        }

        yield return new WaitForSeconds(1f); // Giữ chế độ ngắm

        // --- Gọi animation bắn ---
        animator.SetTrigger("Fire");
        MoveUntil.UseSkill(firePoint, enemy.gameObject);

        // --- Tắt trạng thái ngắm ---
        animator.SetBool("IsAiming", false);
        yield return new WaitForSeconds(0.5f);

        // --- Hạ cánh (mượt) ---
        animator.SetBool("IsFalling", true);
        float fallTime = 0.5f;
        elapsed = 0f;
        Vector3 currentPos = transform.position;
        while (elapsed < fallTime)
        {
            transform.position = Vector3.Lerp(currentPos, originalPosition, elapsed / fallTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = originalPosition;

        // --- Bật lại gravity ---
        animator.SetBool("IsFalling", false);
        gravity = -9.81f;
        ThirdPersonCamera.instance.offset = new Vector3(0, 3, -6f);
        canMove = true;
    }

    private void Move2()
    {
        if (CurrentcooldownMove2 > 0)
        {
            Debug.Log("Secondary move is on cooldown!");
            return;
        }
        Debug.Log("Using Skill: " + secondaryMove.skillData.skillName);
        secondaryMove.UseSkill(firePoint, GetNearestEnemy().gameObject);
        CurrentcooldownMove2 = secondaryMove.skillData.cooldown;
    }
    private IEnumerator FireArrow()
    {
        yield return new WaitForSeconds(0.1f); // Đợi 0.1 giây để tránh spam
        if (controller.isGrounded == false)
        {
            Debug.Log("Cannot fire arrow while in the air!");
            yield break;
        }
        Vector3 direction;      // hướng bắn
        Quaternion rotation;    // quay của mũi tên

        if (isAiming)
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.51f, 0.51f));
            Vector3 targetPoint = ray.origin + ray.direction * 100f;

            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
                targetPoint = hit.point;

            direction = (targetPoint - firePoint.position).normalized;
            rotation = Quaternion.LookRotation(direction);
        }
        else
        {
            // 🔸 KHÔNG AIM → tự chọn enemy gần nhất
            Transform enemy = GetNearestEnemy();
            if (enemy == null)  yield break;             // Không có địch thì thôi

            // Xoay nhân vật chỉ theo trục Y
            Vector3 flatDir = enemy.position - transform.position;
            flatDir.y = 0f;
            if (flatDir != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(flatDir);
                // Quay thêm 90 độ quanh trục Y (tùy model quay thiếu hay dư)
                transform.rotation = lookRotation * Quaternion.Euler(0f, 90f, 0f);
            }

            // Hướng bắn thẳng vào thân địch (nâng nhẹ Y cho tự nhiên)
            direction = (enemy.position + Vector3.up * 1.2f - firePoint.position).normalized;
            rotation = Quaternion.LookRotation(direction);
        }

        // Gọi VFX + Arrow
        Instantiate(VFX, firePoint.position, rotation);
        Instantiate(arrowPrefab, firePoint.position, rotation);

        animator.SetTrigger("Fire");
        Debug.DrawRay(firePoint.position, direction * 30f, Color.red, 1.5f);
    }


    // Tùy ý giới hạn tầm dò nếu muốn (không bắt buộc)
    private Transform GetNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform nearest = null;
        float minSqr = Mathf.Infinity;
        Vector3 myPos = transform.position;

        foreach (GameObject e in enemies)
        {
            float sqr = (e.transform.position - myPos).sqrMagnitude;
            if (sqr < minSqr)
            {
                minSqr = sqr;
                nearest = e.transform;
            }
        }
        return nearest;
    }
    private void HandleAimingWithRigTarget()
    {
        Vector2 lookInput = inputActions.Player.Look.ReadValue<Vector2>();

        float lookX = lookInput.x * sensitivity;
        float lookY = lookInput.y * sensitivity;

        aimYaw += lookX;
        aimPitch -= lookY;

        aimPitch = Mathf.Clamp(aimPitch, -90f, 90f);

        float maxRight = 0f;
        float maxLeft = -40f;

        if (aimYaw > maxRight)
        {
            float excess = aimYaw - maxRight;
            aimYaw = maxRight;
            transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y + excess, 0f);
        }
        else if (aimYaw < maxLeft)
        {
            float excess = aimYaw - maxLeft; // khác chỗ này!
            aimYaw = maxLeft;
            transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y + excess, 0f);
        }


        // Cập nhật vị trí Rig Target
        Vector3 direction = Quaternion.Euler(aimPitch, aimYaw, 0f) * Vector3.forward;
        Vector3 targetPosition = transform.position + transform.rotation * direction * 2f + Vector3.up * 1.5f;
        aimTarget.position = targetPosition;
    }

}
