using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class CharacterControllerInput : MonoBehaviour
{
    protected InputSystem inputActions;
    protected Vector2 moveInput;
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    public float verticalVelocity = 0f;
    public float groundedCheckDistance = 0.1f; // Khoảng cách kiểm tra mặt đất
    public bool canMove = true; // Mặc định được phép di chuyển
    [HideInInspector] public CharacterController controller;
    public Camera mainCamera;
    [HideInInspector]public Animator animator;

    protected virtual void Awake()
    {
        inputActions = new InputSystem();
    }

    protected virtual void Start()
    {
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        animator = GetComponent<Animator>();

        inputActions.Enable();
        inputActions.Player.Move.performed += OnMovePerformed;
        inputActions.Player.Move.canceled += OnMoveCanceled;
        inputActions.Player.Jump.performed += ctx => Jump();
    }

    protected virtual void Update()
    {
        if (!canMove) return; // Không xử lý input nếu đang bị khóa
        Move();
        ApplyGravity();
    }
    protected virtual void ApplyGravity()
    {
        // Nếu đang dưới đất → giữ sát mặt đất hoặc reset vận tốc rơi
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // giữ nhẹ trên mặt đất, không bị dính
        }
        else
        {
            // Áp lực hấp dẫn
            verticalVelocity += gravity * Time.deltaTime;
        }
    }

    protected virtual void Move()
    {
        if (moveInput == Vector2.zero)
        {
            animator?.SetFloat("Speed", 0f);
        }

        Vector3 camForward = mainCamera.transform.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = mainCamera.transform.right;
        camRight.y = 0;
        camRight.Normalize();

        Vector3 moveDir = camForward * moveInput.y + camRight * moveInput.x;
        moveDir.Normalize();

        // Di chuyển ngang
        Vector3 finalMove = moveDir * moveSpeed;

        // Cộng thêm rơi (gravity)
        finalMove.y = verticalVelocity;

        controller.Move(finalMove * Time.deltaTime);

        // Gán animation
        animator?.SetFloat("Speed", moveDir != Vector3.zero ? 1f : 0f);

        // Chỉ xoay nếu không aim
        if (moveDir != Vector3.zero && !(this is Archer archer && archer.IsAiming))
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
        }
    }
    protected virtual void Jump()
    {
        if (controller.isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(-2f * gravity * 1.0f); // 1.0f là chiều cao nhảy
            animator?.SetTrigger("Jump");
        }
    }

    protected virtual void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    protected virtual void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }
}
