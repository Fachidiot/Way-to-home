using AIStates;
using UnityEngine;

public class MonsterStateMachine : BaseStateMachine
{
    private readonly Idle idleState = new Idle();
    private readonly Patrol patrolState = new Patrol();
    private readonly Trace traceState = new Trace();
    private readonly Interact interactState = new Interact();
    private readonly Attack attackState = new Attack();
    private readonly Hit hitState = new Hit();
    private readonly Die dieState = new Die();

    // Basic States 기본상태들
    public override BaseState<MonsterController> IdleState => idleState;            // Idle 상태
    public override BaseState<MonsterController> PatrolState => patrolState;        // 필요한 요소를 찾아 탐색
    public override BaseState<MonsterController> TraceState => traceState;          // 필요한 요소를 찾았다면 요소를 쫓아감
    public override BaseState<MonsterController> InteractState => interactState;    // 요소와의 상호작용
    public override BaseState<MonsterController> AttackState => attackState;        // 전투를 해야하는 상태
    public override BaseState<MonsterController> HitState => hitState;              // 전투에서 맞았을때
    public override BaseState<MonsterController> DieState => dieState;              // 죽었을때.
}