using UnityEngine;
using UnityEngine.AI;

namespace AIStates
{
    /// <summary>
    /// Idle : 가만히 서있는 기본 상태.
    /// [ Enter ] Random 시간 Idle 상태 애니메이션 재생
    /// [ Update ] Sensor.IsOnSight -> Trace
    ///            time end -> Patrol
    ///            + MonsterStates에서 override시켜서 eat, poo, sleep을 해주도록 하면 될듯?
    /// </summary>
    public class Idle : BaseState<MonsterController>
    {
        private float idleTime;
        private float timer;

        public override void EnterState(MonsterController monster)
        {
            Debug.Log($"{monster.name} Idle State Entered");
            monster.StopMoving();
            idleTime = Random.Range(monster.Config.idleMinTime, monster.Config.idleMaxTime);
            timer = 0f;
        }

        public override void ExitState(MonsterController monster) { }

        public override BaseState<MonsterController> UpdateState(MonsterController monster)
        {
            if (monster.sensor.IsOnSight)
            {   // Target 발견시 Trace상태로 변경
                return monster.stateMachine.TraceState;
            }
            timer += Time.deltaTime;
            if (timer >= idleTime)
            {
                return monster.stateMachine.PatrolState;
            }
            return this;
        }
    }

    /// <summary>
    /// Patrol : 탐색할 요소를 찾아서 이동하는 상태.
    /// [ Enter ] RandomPatrolDestination 설정
    ///           Anim -> Walk
    ///           + PatrolType같이 어떤것을 탐색할지 해주는 방법도 낫베드
    /// [ Update ] !friendly + Sensor.isOnSight -> Trace
    ///            !friendly + Sensor.isOnHeard -> Find?
    /// [ Exit ] -> movement.Stop
    /// </summary>
    public class Patrol : BaseState<MonsterController>
    {
        // private PatrolType patrolType;   // 어떤것을 탐색할것인가?
        private Vector3 patrolDestination;
        private float entryTimer;

        public override void EnterState(MonsterController monster)
        {
            Debug.Log($"{monster.name} Patrol State Entered");
            if (monster.sensor.IsOnHeard)   // 무언가 들은게 있다면
                patrolDestination = monster.sensor.TargeLastPosition;
            else    // 아무것도 없다면 랜덤 좌표로 탐색
                patrolDestination = monster.GetPatrolDestination();

            monster.SetAnimFloat(monster.hashMoveSpeed, 1f);
            entryTimer = 0f;
        }

        public override void ExitState(MonsterController monster)
        {
            monster.StopMoving();
            monster.SetAnimFloat(monster.hashMoveSpeed, 0f);
        }

        public override BaseState<MonsterController> UpdateState(MonsterController monster)
        {
            // 우호적이지 않고 플레이어를 발견했거나 소리를 들었다면
            if (!monster.Config.friendly && monster.sensor.IsOnSight)
                return monster.stateMachine.TraceState;  // 시야에 들어왔다면 바로 쫓아가서 싸움

            // 소리만 들었다면 해당 좌표로 순찰
            monster.MoveTo(patrolDestination);
            entryTimer += Time.deltaTime;
            if (entryTimer > 0.1f && monster.arrivedAtDestination)
            {
                return monster.stateMachine.IdleState;
            }

            return this;
        }
    }

    /// <summary>
    /// Trace : Entity가 있다면 해당 적을 쫓아가는 상태.
    /// [ Enter ] Anim -> Run
    /// [ Update ] !friendly or attacked and in attackrange (적대적이거나 공격당했을때(화났을때)) -> AttackState
    ///            target -> Move(target)
    ///            ArrivedToTarget && !Sensor.isOnSight (목적지 도착 & Sensor에 시야에 잡히는게 없을때)
    /// </summary>
    public class Trace : BaseState<MonsterController>
    {
        public override void EnterState(MonsterController monster)
        {
            Debug.Log($"{monster.name} Trace State Entered");
            monster.SetAnimFloat(monster.hashMoveSpeed, 2f);
        }

        public override void ExitState(MonsterController monster)
        {
            if (monster.CurrentState != monster.stateMachine.AttackState)
            // || monster.CurrentState != monster.stateMachine.BlockState)
            {
                monster.SetAnimFloat(monster.hashMoveSpeed, 0f);
            }
        }

        public override BaseState<MonsterController> UpdateState(MonsterController monster)
        {
            if (monster.arrivedAtDestination)
            {
                if (!monster.Config.friendly || monster.attacked)
                {   // 적대적 or 공격받았을때
                    if (monster.GetDistanceToTarget() <= monster.Config.attackRange)
                        return monster.stateMachine.AttackState;
                }

                // 목적지에 도달했으나, target의 센서에서 감지가 되지 않을때.
                if (!monster.sensor.IsOnSight)
                    return monster.stateMachine.IdleState;
            }

            // 목적지에 도달할때까지 Trace
            if (monster.target)
                monster.MoveTo(monster.target.transform.position);
            return this;
        }
    }

    /// <summary>
    /// Attack : 목표 Entity에게 공격을 가하는 상태
    /// [ Enter ] Anim -> Attack
    ///           StopMoving, LookAt(target)
    /// [ Update ] StopMoving, LookAt(target)
    /// </summary>
    public class Attack : BaseState<MonsterController>
    {
        private float timer;

        public override void EnterState(MonsterController monster)
        {
            Debug.Log($"{monster.name} Attack State Entered");
            monster.StopMoving();
            monster.SetAnimTrigger(monster.hashAttack1);

            LookAt(monster);

            timer = 0;
        }

        public override void ExitState(MonsterController monster) { monster.StopAllCoroutines(); }

        public override BaseState<MonsterController> UpdateState(MonsterController monster)
        {
            monster.StopMoving();

            LookAt(monster);

            timer += Time.deltaTime;
            if (timer >= monster.Config.attackTimeout)
                return monster.stateMachine.TraceState;

            return this;
        }

        private void LookAt(MonsterController monster)
        {
            if (monster.target)
                monster.LookAt(monster.target.transform.position);
        }
    }

    public class Interact : BaseState<MonsterController>
    {
        public override void EnterState(MonsterController monster)
        {
            Debug.Log($"{monster.name} Interact State Entered");
            throw new System.NotImplementedException();
        }

        public override void ExitState(MonsterController monster) { }

        public override BaseState<MonsterController> UpdateState(MonsterController monster)
        {
            throw new System.NotImplementedException();
        }
    }

    public class Hit : BaseState<MonsterController>
    {
        private float hitStunDuration = 0.5f;
        private float timer;

        public override void EnterState(MonsterController monster)
        {
            Debug.Log($"{monster.name} Hit");
            timer = 0f;
            if (monster.TryGetComponent<NavMeshAgent>(out var agent))
                agent.speed = monster.Config.runSpeed * 0.3f;
        }

        public override void ExitState(MonsterController monster) { }

        public override BaseState<MonsterController> UpdateState(MonsterController monster)
        {
            timer += Time.deltaTime;
            if (timer > -hitStunDuration)
                return monster.stateMachine.TraceState;
            return this;
        }
    }

    public class Die : BaseState<MonsterController>
    {
        public override void EnterState(MonsterController monster)
        {
            Debug.Log($"{monster.name} Dead");
            monster.StopMoving();
            monster.StopAllCoroutines();

            // TODO : 추후 AIController에 Die코드 만들어서 실행해줘도 될것 같음.
            if (monster.TryGetComponent<Collider>(out var collider))
                collider.enabled = false;

            if (monster.TryGetComponent<NavMeshAgent>(out var agent))
                agent.enabled = false;

            // Sample Loot Logic
            // MonsterHealth health = monster.GetComponent<MonsterHealth>();
            // if (!health)// && !monster.Config.lootTable
            //     health.SpawnLoot();// monster.Config.lootTable);

            // TODO : Network Logic here
            // if (!MonsterManager.Instance)
            //     MonsterManager.Instance.RegisterMonsterDied();
        }

        public override void ExitState(MonsterController monster) { }

        public override BaseState<MonsterController> UpdateState(MonsterController monster)
        {
            return this;
        }
    }

}