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
            monster.SetAnimBool(monster.hashIsWalking, false);
            monster.SetAnimBool(monster.hashIsRunning, false);
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

        public override void EnterState(MonsterController Monster)
        {
            Debug.Log($"{Monster.name} Patrol State Entered");
            if (Monster.sensor.IsOnHeard)   // 무언가 들은게 있다면
                patrolDestination = Monster.sensor.TargeLastPosition;
            else    // 아무것도 없다면 랜덤 좌표로 탐색
                patrolDestination = Monster.GetPatrolDestination();

            Monster.SetAnimFloat(Monster.hashMoveSpeed, 1f);
            entryTimer = 0f;
        }

        public override void ExitState(MonsterController Monster)
        {
            Monster.StopMoving();
            Monster.SetAnimFloat(Monster.hashMoveSpeed, 0f);
        }

        public override BaseState<MonsterController> UpdateState(MonsterController Monster)
        {
            // 우호적이지 않고 플레이어를 발견했거나 소리를 들었다면
            if (!Monster.Config.friendly && Monster.sensor.IsOnSight)
                return Monster.stateMachine.TraceState;  // 시야에 들어왔다면 바로 쫓아가서 싸움

            // 소리만 들었다면 해당 좌표로 순찰
            Monster.MoveTo(patrolDestination);
            entryTimer += Time.deltaTime;
            if (entryTimer > 0.1f && Monster.arrivedAtDestination)
            {
                return Monster.stateMachine.IdleState;
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
        public override void EnterState(MonsterController Monster)
        {
            Debug.Log($"{Monster.name} Trace State Entered");
            Monster.SetAnimFloat(Monster.hashMoveSpeed, 2f);
        }

        public override void ExitState(MonsterController Monster)
        {
            if (Monster.CurrentState != Monster.stateMachine.AttackState)
            // || Monster.CurrentState != Monster.stateMachine.BlockState)
            {
                Monster.SetAnimFloat(Monster.hashMoveSpeed, 0f);
            }
        }

        public override BaseState<MonsterController> UpdateState(MonsterController Monster)
        {
            if (Monster.arrivedAtDestination)
            {
                if (!Monster.Config.friendly || Monster.attacked)
                {   // 적대적 or 공격받았을때
                    if (Monster.GetDistanceToTarget() <= Monster.Config.attackRange)
                        return Monster.stateMachine.AttackState;
                }

                // 목적지에 도달했으나, target의 센서에서 감지가 되지 않을때.
                if (!Monster.sensor.IsOnSight)
                    return Monster.stateMachine.IdleState;
            }

            // 목적지에 도달할때까지 Trace
            if (Monster.target)
                Monster.MoveTo(Monster.target.transform.position);
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

        public override void EnterState(MonsterController Monster)
        {
            Debug.Log($"{Monster.name} Attack State Entered");
            Monster.StopMoving();
            Monster.SetAnimTrigger(Monster.hashAttack1);

            LookAt(Monster);

            timer = 0;
        }

        public override void ExitState(MonsterController Monster) { Monster.StopAllCoroutines(); }

        public override BaseState<MonsterController> UpdateState(MonsterController Monster)
        {
            Monster.StopMoving();

            LookAt(Monster);

            timer += Time.deltaTime;
            if (timer >= Monster.Config.attackTimeout)
                return Monster.stateMachine.TraceState;

            return this;
        }

        private void LookAt(MonsterController Monster)
        {
            if (Monster.target)
                Monster.LookAt(Monster.target.transform.position);
        }
    }

    public class Interact : BaseState<MonsterController>
    {
        public override void EnterState(MonsterController Monster)
        {
            Debug.Log($"{Monster.name} Interact State Entered");
            throw new System.NotImplementedException();
        }

        public override void ExitState(MonsterController Monster) { }

        public override BaseState<MonsterController> UpdateState(MonsterController Monster)
        {
            throw new System.NotImplementedException();
        }
    }

    public class Hit : BaseState<MonsterController>
    {
        private float hitStunDuration = 0.5f;
        private float timer;

        public override void EnterState(MonsterController Monster)
        {
            Debug.Log($"{Monster.name} Hit");
            timer = 0f;
            if (Monster.TryGetComponent<NavMeshAgent>(out var agent))
                agent.speed = Monster.Config.runSpeed * 0.3f;
        }

        public override void ExitState(MonsterController Monster) { }

        public override BaseState<MonsterController> UpdateState(MonsterController Monster)
        {
            timer += Time.deltaTime;
            if (timer > -hitStunDuration)
                return Monster.stateMachine.TraceState;
            return this;
        }
    }

    public class Die : BaseState<MonsterController>
    {
        public override void EnterState(MonsterController Monster)
        {
            Debug.Log($"{Monster.name} Dead");
            Monster.StopMoving();
            Monster.StopAllCoroutines();

            // TODO : 추후 AIController에 Die코드 만들어서 실행해줘도 될것 같음.
            if (Monster.TryGetComponent<Collider>(out var collider))
                collider.enabled = false;

            if (Monster.TryGetComponent<NavMeshAgent>(out var agent))
                agent.enabled = false;

            // Sample Loot Logic
            // MonsterHealth health = Monster.GetComponent<MonsterHealth>();
            // if (!health)// && !Monster.Config.lootTable
            //     health.SpawnLoot();// Monster.Config.lootTable);

            // TODO : Network Logic here
            // if (!MonsterManager.Instance)
            //     MonsterManager.Instance.RegisterMonsterDied();
        }

        public override void ExitState(MonsterController Monster) { }

        public override BaseState<MonsterController> UpdateState(MonsterController Monster)
        {
            return this;
        }
    }

}