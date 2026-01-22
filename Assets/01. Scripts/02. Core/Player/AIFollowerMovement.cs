using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIFollowerMovement : MonoBehaviour
{
    [SerializeField] private float stoppingDistance = 1f;

    private Animator animator;
    private NavMeshAgent agent;
    private Transform target;

    bool isSprint;
    bool isCrouch;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        AgentSetup();
    }

    void AgentSetup()
    {
        var controller = GetComponent<CharacterController>();
        agent.radius = controller.radius;
        agent.height = controller.height;
        agent.stoppingDistance = stoppingDistance;
    }

    public void ChangeTarget(Transform destination)
    {
        target = destination;
    }

    public void SetAnimation(bool isSprint, bool isCrouch)
    {
        this.isSprint = isSprint;
        this.isCrouch = isCrouch;
    }

    void Update()
    {
        agent.SetDestination(target.position);
        animator.SetFloat("velocity", agent.velocity.sqrMagnitude);

        animator.SetBool("isSprint", isSprint);
        animator.SetBool("isCrouch", isCrouch);
        CalculateSpeed();
    }

    void CalculateSpeed()
    {
        float distance = (target.position - transform.position).sqrMagnitude;

        float speed = distance > 3 ? 5 : 2;

        agent.speed = speed;
    }
}
