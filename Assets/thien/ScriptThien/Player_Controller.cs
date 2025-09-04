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

    // === Movement Control ===
    public bool canMove = true; // Allow external control of movement

    void Start()
    {
        currentMoveSpeed = moveSpeed;
        VFXchay.SetActive(false);
    }

    void Update()
    {
        if (isRolling || !canMove) return; // Add canMove check

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
        if (!canMove) return; // Prevent sprinting when movement is disabled

        bool wantToSprint = (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
                      Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
                      && Input.GetKey(KeyCode.LeftShift);

        if (wantToSprint && !isSprinting)
        {
            isSprinting = true;
            VFXchay.SetActive(true);
            animator.speed = 1.5f;
        }
        else if (!wantToSprint && isSprinting)
        {
            isSprinting = false;
            currentMoveSpeed = moveSpeed;
            animator.speed = 1f;
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
        if (!canMove) // Prevent movement when disabled
        {
            animator.SetBool("isRun", false);
            return;
        }

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
        if (!canMove) return; // Prevent jumping when movement is disabled

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        animator.SetTrigger("jump");
        isGrounded = false;
    }

    IEnumerator PerformRoll()
    {
        if (!canMove) yield break; // Prevent rolling when movement is disabled

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

        if (isMoving && canMove) // Add canMove check
        {
            animator.SetBool("isRun", true);

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

    // Public method to control movement from other scripts
    public void SetMovementEnabled(bool enabled)
    {
        canMove = enabled;
        if (!enabled)
        {
            animator.SetBool("isRun", false);
            isSprinting = false;
            VFXchay.SetActive(false);
            animator.speed = 1f;
        }
    }
}