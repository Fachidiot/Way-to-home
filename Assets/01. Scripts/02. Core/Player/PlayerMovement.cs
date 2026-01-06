using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    [Tooltip("모델읠 Transform의 rotation을 회전시켜줍니다.")]
    [SerializeField] private Transform model;
    private Animator animator;

    [Header("Player Settings")]
    [Header("Speed Settings")]
    [SerializeField] private float crouchSpeed = 0.5f;
    [SerializeField] private float walkSpeed = 1f;
    [SerializeField] private float sprintSpeed = 5f;
    [SerializeField] private float acceleration = 0.1f;
    [SerializeField] private float currentSpeed;
    private bool isGrounded;
    private bool isCrouched;
    private bool isSprint;

    [Header("Rotate Settings")]
    [SerializeField] private float rotateSpeed = 5f;
    [SerializeField] private float quickTurnMinAngle = 135f;
    [SerializeField] private float quickTurnTimeout = 0.50f;
    [SerializeField] private float quickTurnTimeoutDelta;

    [Header("Jump And Gravity")]
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private float groundOffset = -0.14f;
    [SerializeField] private float groundCheckSphereRadius = 0.1f;
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpTimeout = 1.50f;
    private float jumpTimeoutDelta;

    [Header("Colider values")]
    [SerializeField] private float crouchColliderHeight = 1f;
    private float normalColliderHeight;

    [SerializeField] private bool debug = false;

    private CharacterController controller;
    private PlayerInputs playerInput;

    private Vector3 velocity;
    private Vector3 prevVelocity = Vector3.zero;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInputs>();
        animator = model.GetComponent<Animator>();

        normalColliderHeight = controller.height;
    }

    void FixedUpdate()
    {
        GroundCheck();
    }

    void Update()
    {
        Move();
        // BodyTurn();
        Rotate();
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
        // 방향 계산
        Vector2 direction;
        if (isGrounded)
            direction = new Vector2(playerInput.move.x, playerInput.move.y);
        else
            direction = new Vector2(prevVelocity.x, prevVelocity.z);

        var isMoving = direction.magnitude > 0;

        // targetSpeed 계산
        var targetSpeed = SpeedChange(isMoving);
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 5);

        // velocity 계산
        Quaternion moveForward = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);  // TODO : 추후 이놈 때문에 transform.rotation을 회전시켰을때 이동이 이상해지는지 확인해주세요.
        Vector3 rawMoveDirection = (moveForward * Vector3.forward * direction.y)
                                + (moveForward * Vector3.right * direction.x);

        // 이전 velocity에서의 Turn Anlge이 quick상태라면 quickturn을 실행 및 가속도 적용.
        Vector3 horizontalVelocity;
        if (Vector3.zero != prevVelocity && BodyTurn())
        {
            if (isGrounded && currentSpeed > (walkSpeed + 0.5) && quickTurnTimeoutDelta <= 0.0f)
            {   // 달리는 상태 + Quicktrun 상태라면 Animation 재생
                animator.SetTrigger("sprintTurnRight");
                quickTurnTimeoutDelta = quickTurnTimeout;
            }

            // 이전 속도 -> 천천히 감속해준다.
            currentSpeed = Mathf.Lerp(0, currentSpeed, Time.deltaTime * 5);
            // 감속된 속도에서 이동 방향으로의 보정.
            horizontalVelocity = Vector3.Lerp(prevVelocity, rawMoveDirection.normalized * currentSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            horizontalVelocity = rawMoveDirection.normalized * currentSpeed;

            quickTurnTimeoutDelta -= Time.deltaTime;
        }
        prevVelocity = horizontalVelocity;
        Vector3 verticalVelocity = new Vector3(0, velocity.y, 0);

        if (isGrounded && verticalVelocity.y > gravity * Time.deltaTime)
        {   // isGrounded일 때는 중력을 계속 적용하여 경사면에서 뜨지 않도록 함
            verticalVelocity.y = -2f;
        }
        velocity = horizontalVelocity + verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
        animator.SetFloat("velocity", direction.magnitude);
    }

    bool BodyTurn()
    {
        // Player가 가는 방향 : model의 forward방향에서 현재 direction과의 angle구하기.
        // angle이 quickturnAngle값보다 크다면 animation 실행.
        bool quickTurn = false;
        var direction = new Vector3(playerInput.move.x, 0, playerInput.move.y);
        float angle = Vector3.Angle(model.forward, direction);

        if (isSprint && angle > quickTurnMinAngle)
        {
            // Debug.Log($"{angle} > {quickTurnMinAngle}");
            quickTurn = true;
        }
        if (!isSprint && !isCrouched && angle > quickTurnMinAngle)
        {
            // TODO: walk Degreeturn Animation
            quickTurn = true;
        }
        if (isGrounded && Vector3.zero != direction && angle > quickTurnMinAngle - 10)
            model.forward = Vector3.Lerp(model.forward, direction, Time.deltaTime * rotateSpeed * 2);
        return quickTurn;
    }

    private void Jump()
    {
        if (isGrounded)
        {   // On Ground
            animator.SetBool("isJump", false);

            if (jumpTimeoutDelta > 0.0f)
                playerInput.jump = false;

            // Jump
            if (isGrounded && playerInput.jump && jumpTimeoutDelta <= 0.0f)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);

                animator.SetBool("isJump", true);
            }

            if (jumpTimeoutDelta >= 0.0f)
                jumpTimeoutDelta -= Time.deltaTime;
        }
        else
        {   // On Air
            jumpTimeoutDelta = jumpTimeout;
            velocity.y += gravity * Time.deltaTime;

            playerInput.jump = false;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    void Rotate()
    {
        if (!isGrounded)
            return;
        var direction = new Vector3(playerInput.move.x, 0, playerInput.move.y);
        if (Vector3.zero != direction)
            model.forward = Vector3.Lerp(model.forward, direction, Time.deltaTime * rotateSpeed);
    }

    private float SpeedChange(bool isMoving)
    {
        isSprint = playerInput.sprint;
        animator.SetBool("isSprint", isSprint);

        isCrouched = playerInput.crouch && !playerInput.sprint;
        animator.SetBool("isCrouch", isCrouched);

        if (isCrouched)
            controller.height = crouchColliderHeight;
        else
            controller.height = normalColliderHeight;

        controller.center = new Vector3(
            controller.center.x,
            controller.height / 2 + 0.05f,
            controller.center.z);

        return isMoving ? (isSprint ? sprintSpeed : (isCrouched ? crouchSpeed : walkSpeed)) : 0;
    }

    private void OnDrawGizmos()
    {
        if (!debug)
            return;

        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y + groundOffset, transform.position.z);
        Gizmos.DrawSphere(spherePosition, groundCheckSphereRadius);
    }
}
