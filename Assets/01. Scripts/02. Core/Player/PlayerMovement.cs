using Data;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private PlayerMovementConfig config;

    private Animator animator;
    [SerializeField] private bool isGrounded;
    private bool isCrouched;
    private bool isSprint;
    private float currentSpeed;
    private float jumpTimeoutDelta;
    private float normalColliderHeight;

    [SerializeField] private bool debug = false;

    private CharacterController controller;
    private PlayerInputs playerInput;

    private Vector3 velocity;
    private Vector3 prevVelocity = Vector3.zero;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GameManager.Instance.GetComponent<PlayerInputs>();
        animator = GetComponentInChildren<Animator>();

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
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y + config.groundOffset, transform.position.z);
        isGrounded = Physics.CheckSphere(spherePosition, config.groundCheckSphereRadius, config.groundLayers, QueryTriggerInteraction.Ignore);

        animator.SetBool("isGrounded", isGrounded);
    }

    void Move()
    {
        // 방향 계산
        Vector2 rawdirection;
        if (isGrounded)
            rawdirection = new Vector2(playerInput.move.x, playerInput.move.y);
        else
            rawdirection = new Vector2(prevVelocity.x, prevVelocity.z);
        Vector2 direction = new Vector2(0, math.abs(playerInput.move.x) + math.abs(playerInput.move.y));

        var isMoving = rawdirection.magnitude > 0;

        // targetSpeed 계산
        var targetSpeed = SpeedChange(isMoving);
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 5);

        // velocity 계산
        Quaternion moveForward = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        // TODO : 추후 이놈 때문에 transform.rotation을 회전시켰을때 이동이 이상해지는지 확인해주세요
        Vector3 forwardDirection = (moveForward * Vector3.forward * direction.y)
                                + (moveForward * Vector3.right * direction.x);

        // 이전 velocity에서의 Turn Anlge이 quick상태라면 quickturn을 실행 및 가속도 적용.
        Vector3 horizontalVelocity;
        if (Vector3.zero != prevVelocity && QuickTurn())
        {
            if (isGrounded && currentSpeed > (config.walkSpeed + 0.5) && config.quickTurnTimeoutDelta <= 0.0f)
            {   // 달리는 상태 + Quicktrun 상태라면 Animation 재생
                animator.SetTrigger("sprintTurnRight");
                config.quickTurnTimeoutDelta = config.quickTurnTimeout;
            }

            // 이전 속도 -> 천천히 감속해준다.
            currentSpeed = Mathf.Lerp(0, currentSpeed, Time.deltaTime * 5);
            // 감속된 속도에서 이동 방향으로의 보정.
            horizontalVelocity = Vector3.Lerp(prevVelocity, forwardDirection.normalized * currentSpeed, config.acceleration * Time.deltaTime);
        }
        else
        {
            horizontalVelocity = forwardDirection.normalized * currentSpeed;

            if (config.quickTurnTimeoutDelta > 0)
                config.quickTurnTimeoutDelta -= Time.deltaTime;
        }
        if (isGrounded)
            prevVelocity = horizontalVelocity;
        Vector3 verticalVelocity = new Vector3(0, velocity.y, 0);

        if (isGrounded && verticalVelocity.y > config.gravity * Time.deltaTime)
        {   // isGrounded일 때는 중력을 계속 적용하여 경사면에서 뜨지 않도록 함
            verticalVelocity.y = -2f;
        }

        if (isGrounded)
            velocity = horizontalVelocity + verticalVelocity;
        else
            velocity = prevVelocity + verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
        animator.SetFloat("velocity", rawdirection.magnitude);
    }

    bool QuickTurn()
    {
        // Player가 가는 방향 : model의 forward방향에서 현재 direction과의 angle구하기.
        // angle이 quickturnAngle값보다 크다면 animation 실행.
        bool quickTurn = false;
        var direction = new Vector3(playerInput.move.x, 0, playerInput.move.y);
        float angle = Vector3.Angle(transform.forward, direction);

        if (isSprint && angle > config.quickTurnMinAngle)
        {
            // Debug.Log($"{angle} > {quickTurnMinAngle}");
            quickTurn = true;
        }
        if (!isSprint && !isCrouched && angle > config.quickTurnMinAngle)
        {
            // TODO: walk Degreeturn Animation
            quickTurn = true;
        }
        if (isGrounded && Vector3.zero != direction && angle > config.quickTurnMinAngle - 10)
            transform.forward = Vector3.Lerp(transform.forward, direction, Time.deltaTime * config.rotateSpeed * 2);
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
                velocity.y = Mathf.Sqrt(config.jumpHeight * -2 * config.gravity);

                animator.SetBool("isJump", true);
            }

            if (jumpTimeoutDelta >= 0.0f)
                jumpTimeoutDelta -= Time.deltaTime;
        }
        else
        {   // On Air
            jumpTimeoutDelta = config.jumpTimeout;
            velocity.y += config.gravity * Time.deltaTime;

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
            transform.forward = Vector3.Lerp(transform.forward, direction, Time.deltaTime * config.rotateSpeed);
    }

    private float SpeedChange(bool isMoving)
    {
        isSprint = playerInput.sprint;
        animator.SetBool("isSprint", isSprint);

        isCrouched = playerInput.crouch && !playerInput.sprint;
        animator.SetBool("isCrouch", isCrouched);

        /// TODO : 추후 성능에 문제가 있다면 확인.
        GameManager.Instance.SetMainAnimation(isSprint, isCrouched);

        if (isCrouched)
            controller.height = config.crouchColliderHeight;
        else
            controller.height = normalColliderHeight;

        controller.center = new Vector3(
            controller.center.x,
            controller.height / 2 + 0.05f,
            controller.center.z);

        return isMoving ? (isSprint ? config.sprintSpeed : (isCrouched ? config.crouchSpeed : config.walkSpeed)) : 0;
    }

    private void OnDrawGizmos()
    {
        if (!debug)
            return;

        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y + config.groundOffset, transform.position.z);
        Gizmos.DrawSphere(spherePosition, config.groundCheckSphereRadius);
    }
}
