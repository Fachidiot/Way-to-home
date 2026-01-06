using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "MonsterStats", menuName = "DungBeetle/Settings/MonsterStats/MonsterStats", order = 1)]
    public class MonsterStats : ScriptableObject
    {
        [Header("Ability Settings")]
        public float maxHP = 100f;
        public float attackDamage = 10f;
        public float defense = 0f;

        [Header("AI Settings")]
        public float fovRange = 10f;
        [Range(0, 360)]
        public float fovAngle = 120f;
        public float attackRange = 2f;
        public float stoppingDistance = 1.5f;
        public float soundRange = 15f;

        // [Header("Patrol Settings")]
        [Header("Speed Settings")]
        public float walkSpeed = 1.5f;
        public float runSpeed = 3f;
        public float rotateSpeed = 30f;

        [Header("Patrol Settings")]
        public float attackDelay = 0.5f;
        public float attackTimeout = 2f;
    }
}