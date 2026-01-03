using System.ComponentModel;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private Transform rig;

    private CharacterController characterController;
    private PlayerInputs playerInput;
    private Animator animator;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInputs>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
        Rotate();
    }

    void Move()
    {
        Vector3 movement = new Vector3(playerInput.move.x, 0, playerInput.move.y) * speed * Time.deltaTime;
        characterController.Move(movement);

        Debug.Log($"{characterController.velocity.magnitude}");
        animator.SetFloat("velocity", characterController.velocity.normalized.magnitude);
    }

    void Rotate()
    {   // x, z축으로 이동시 회전
        // z + 0 z - 180 x + 90 x - 270
        float rotationVelocity = 0f;
        Vector3 velocity = characterController.velocity.normalized;
        float targetRotation = velocity.x > 0 ? 90 : velocity.x < 0 ? 270 : 0;
        targetRotation += velocity.z < 0 ? 180 : 0;
        float rotation = Mathf.SmoothDampAngle(rig.eulerAngles.y, targetRotation, ref rotationVelocity, 0.2f);
        Debug.Log($"{rotation}");
        rig.transform.localEulerAngles = new Vector3(0, rotation, 0);
    }
}
