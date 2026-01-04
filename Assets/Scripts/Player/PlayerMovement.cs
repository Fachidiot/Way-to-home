using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private float currentSpeed;
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float sprintSpeed = 5f;

    [Header("Jump And Gravity")]
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private float groundOffset = -0.14f;
    [SerializeField] private float groundCheckSphereRadius = 0.1f;
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpTimeout = 0.50f;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isJumped;
    private float jumpTimeoutDelta;

    [Header("Colider values")]
    public float crouchColliderHeight = 1f;
    public float normalColliderHeight { get; private set; }
    public float grounCheckDistance;

    private CharacterController controller;
    private PlayerInputs playerInput;
    private Animator animator;

    private Vector3 velocity;
    private bool isSprint;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInputs>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        GroundCheck();
        Move();
    }

    void Update()
    {
        Jump();
    }

    void GroundCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y + groundOffset, transform.position.z);
        isGrounded = Physics.CheckSphere(spherePosition, groundCheckSphereRadius, groundLayers, QueryTriggerInteraction.Ignore);

        animator.SetBool("isGrounded", isGrounded);
    }

    void Move()
    {
        var inputVector = new Vector2(playerInput.move.x, playerInput.move.y);
        isSprint = playerInput.sprint && inputVector.y > 0;

        var isMoving = inputVector.magnitude > 0;
        float targetSpeed = isMoving ? (isSprint ? sprintSpeed : walkSpeed) : 0;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 5);

        Quaternion moveForward = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        Vector3 rawMoveDirection = (moveForward * Vector3.forward * inputVector.y) + (moveForward * Vector3.right * inputVector.x);

        Vector3 horizontalVelocity = rawMoveDirection.normalized * currentSpeed;

        // isGrounded일 때는 중력을 계속 적용하여 경사면에서 뜨지 않도록 함
        Vector3 verticalVelocity = new Vector3(0, velocity.y, 0);
        if (isGrounded && verticalVelocity.y > gravity * Time.deltaTime)
        {
            verticalVelocity.y = -2f;
        }

        velocity = horizontalVelocity + verticalVelocity;
        controller.Move(velocity * Time.deltaTime);
    }

    private void Jump()
    {
        if (isGrounded)
        {   // On Ground
            animator.SetBool("jump", false);

            if (jumpTimeoutDelta > 0.0f)
                playerInput.jump = false;

            // Jump
            if (isGrounded && playerInput.jump && jumpTimeoutDelta <= 0.0f)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
                // controller.Move(velocity * Time.deltaTime);

                animator.SetBool("jump", true);
            }

            if (jumpTimeoutDelta >= 0.0f)
                jumpTimeoutDelta -= Time.deltaTime;
        }
        else
        {   // On Air
            jumpTimeoutDelta = jumpTimeout;

            playerInput.jump = false;
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y + groundOffset, transform.position.z);
        Gizmos.DrawSphere(spherePosition, groundCheckSphereRadius);
    }
}
