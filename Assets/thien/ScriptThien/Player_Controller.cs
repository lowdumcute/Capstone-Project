using UnityEngine;
using System.Collections;

public class Player_Controller : MonoBehaviour
{
    // === Basic Settings ===
    public Rigidbody rb;
    public Animator animator;
    public float moveSpeed = 9f;
    public float jumpForce = 5f;
    public float rollDistance = 5f;
    public float rollDuration = 0.8f;

    // === Sprint Settings ===
    public float maxSprintSpeed = 15f;
    public float sprintAcceleration = 1f;
    public float currentMoveSpeed;
    private bool isSprinting = false;

    // === State ===
    private bool isGrounded = true;
    private bool isRolling = false;
    private bool canRoll = true;
    private bool wasRunningBeforeRoll = false;
    private Vector3 rollTargetPosition;
    public GameObject VFXchay;
    [SerializeField] private string attack1Animation = "idle";

    void Start()
    {
        currentMoveSpeed = moveSpeed;
        VFXchay.SetActive(false);
    }

    void Update()
    {
        if (isRolling) return;

        HandleSprintInput();
        HandleMovement();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.C) && canRoll && !isRolling)
        {
            StartCoroutine(PerformRoll());
        }
    }

    void HandleSprintInput()
    {
        bool wantToSprint = Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A)&& Input.GetKey(KeyCode.S)&& Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.LeftShift);

        if (wantToSprint && !isSprinting)
        {
            isSprinting = true;
            VFXchay.SetActive(true);
            animator.speed = 1.5f; // Tăng tốc animation lên gấp đôi
        }
        else if (!wantToSprint && isSprinting)
        {
            isSprinting = false;
            currentMoveSpeed = moveSpeed;
            animator.speed = 1f; // Trở về tốc độ bình thường
            VFXchay.SetActive(false);
        }

        if (isSprinting && currentMoveSpeed < maxSprintSpeed)
        {
            currentMoveSpeed += sprintAcceleration * Time.deltaTime;
            currentMoveSpeed = Mathf.Min(currentMoveSpeed, maxSprintSpeed);
        }
    }

    
    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0, vertical).normalized;
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0;
        movement = Quaternion.LookRotation(cameraForward) * movement;

        if (movement.magnitude > 0.1f)
        {
            animator.SetBool("isRun", true);
            wasRunningBeforeRoll = true;

            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
        else
        {
            animator.SetBool("isRun", false);
            wasRunningBeforeRoll = false;
        }

        rb.MovePosition(transform.position + movement * currentMoveSpeed * Time.deltaTime);
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        animator.SetTrigger("jump");
        isGrounded = false;
    }

    IEnumerator PerformRoll()
    {
        isRolling = true;
        canRoll = false;
        wasRunningBeforeRoll = animator.GetBool("isRun");

        animator.SetTrigger("roll_front");
        animator.SetBool("isRun", false);

        rollTargetPosition = transform.position + transform.forward * rollDistance;

        float elapsedTime = 0f;
        Vector3 startingPosition = transform.position;

        while (elapsedTime < rollDuration)
        {
            transform.position = Vector3.Lerp(startingPosition, rollTargetPosition, (elapsedTime / rollDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = rollTargetPosition;

        isRolling = false;
        canRoll = true;

        // Kiểm tra input ngay sau khi roll xong
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool isMoving = (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f);

        if (isMoving)
        {
            // Nếu đang di chuyển thì chuyển sang trạng thái chạy
            animator.SetBool("isRun", true);

            // Nếu đang giữ phím Shift thì bật sprint lại
            if (Input.GetKey(KeyCode.LeftShift))
            {
                isSprinting = true;
                VFXchay.SetActive(true);
                animator.speed = 1.5f;
            }
        }
        else
        {
           
             Invoke("PlayAnimation", 0.02f);
        }
    }
    void PlayAnimation()
    {
        animator.Play(attack1Animation);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}