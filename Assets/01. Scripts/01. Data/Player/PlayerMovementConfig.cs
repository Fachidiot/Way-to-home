using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "MovementConfig", menuName = "Settings/Player/MovementConfig", order = 1)]
    public class PlayerMovementConfig : ScriptableObject
    {
        [Header("Speed Settings")]
        public float crouchSpeed = 0.5f;
        public float walkSpeed = 1f;
        public float sprintSpeed = 3f;
        public float acceleration = 0.1f;

        [Header("Rotate Settings")]
        public float rotateSpeed = 15f;
        public float quickTurnMinAngle = 178f;
        public float quickTurnTimeout = 0.5f;
        public float quickTurnTimeoutDelta;

        [Header("Jump And Gravity")]
        public LayerMask groundLayers;
        public float groundOffset = 0f;
        public float groundCheckSphereRadius = 0.25f;
        public float jumpHeight = 1f;
        public float gravity = -9.81f;
        public float jumpTimeout = 1.3f;

        [Header("Colider values")]
        public float crouchColliderHeight = 1f;
    }

}