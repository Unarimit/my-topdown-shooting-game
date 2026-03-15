using Assets.Scripts.CombatLogic.Characters.Computer.Agent;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.CombatLogic.Characters.BehaviorTreeExtend
{
    /// <summary>
    /// 增强版向敌人移动任务 - 添加了卡住检测和恢复机制
    /// </summary>
    [TaskCategory("AI/Actions")]
    [TaskDescription("Move towards enemy with stuck detection and recovery")]
    internal class MoveToEnemyEnhancedTask : Action
    {
        private AgentController _agentController;
        private NavMeshAgent _navMeshAgent;

        [UnityEngine.Tooltip("卡主检测时间(秒)")]
        public float stuckThreshold = 2f;

        [UnityEngine.Tooltip("卡主时随机移动距离")]
        public float unstuckMoveDistance = 5f;

        [UnityEngine.Tooltip("最大执行时间(秒)")]
        public float maxExecutionTime = 15f;

        // 内部状态
        private Vector3 _lastPosition;
        private float _stuckTimer;
        private float _startTime;
        private int _pathFailCount;
        private const int MAX_PATH_FAILS = 3;

        public override void OnStart()
        {
            _agentController = GetComponent<AgentController>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _lastPosition = transform.position;
            _stuckTimer = 0f;
            _startTime = Time.time;
            _pathFailCount = 0;

            SetDestinationToEnemy();
        }

        public override TaskStatus OnUpdate()
        {
            if (_agentController == null || _navMeshAgent == null)
                return TaskStatus.Failure;

            // 超时检查
            if (Time.time - _startTime > maxExecutionTime)
            {
                return TaskStatus.Success; // 超时，让行为树尝试其他策略
            }

            var enemy = _agentController.GetNearestEnemy();
            if (enemy == null || CombatContextManager.Instance.Operators[enemy.transform].IsDead)
                return TaskStatus.Failure;

            float attackRange = _agentController.Model.AttackRange;
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            // 到达攻击范围
            if (distance <= attackRange)
            {
                return TaskStatus.Success;
            }

            // 卡住检测
            float movedDistance = Vector3.Distance(transform.position, _lastPosition);
            if (movedDistance < 0.2f)
            {
                _stuckTimer += Time.deltaTime;

                // 卡住了，尝试恢复
                if (_stuckTimer >= stuckThreshold)
                {
                    AttemptUnstuck();
                    _stuckTimer = 0f;
                }
            }
            else
            {
                _stuckTimer = 0f;
            }
            _lastPosition = transform.position;

            // 检查路径状态
            if (!_navMeshAgent.hasPath && !_navMeshAgent.pathPending)
            {
                // 没有路径，尝试重新设置
                if (!SetDestinationToEnemy())
                {
                    _pathFailCount++;
                    if (_pathFailCount >= MAX_PATH_FAILS)
                    {
                        return TaskStatus.Failure; // 多次失败，放弃
                    }
                }
            }

            // 定期更新目标位置（敌人可能移动了）
            if (_navMeshAgent.remainingDistance < 2f || Time.frameCount % 30 == 0)
            {
                SetDestinationToEnemy();
            }

            return TaskStatus.Running;
        }

        private bool SetDestinationToEnemy()
        {
            var enemy = _agentController.GetNearestEnemy();
            if (enemy == null) return false;

            Vector3 enemyPos = enemy.transform.position;
            float attackRange = _agentController.Model.AttackRange;

            // 计算一个合理的接近位置（保持在攻击范围内）
            Vector3 direction = (transform.position - enemyPos).normalized;
            if (direction == Vector3.zero)
            {
                direction = Random.insideUnitSphere;
                direction.y = 0;
                direction = direction.normalized;
            }

            // 目标位置：在攻击范围内
            Vector3 targetPos = enemyPos + direction * (attackRange * 0.8f);

            // 确保在NavMesh上
            if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, attackRange * 2f, NavMesh.AllAreas))
            {
                NavMeshPath path = new NavMeshPath();
                if (_navMeshAgent.CalculatePath(hit.position, path))
                {
                    if (path.status == NavMeshPathStatus.PathComplete ||
                        path.status == NavMeshPathStatus.PathPartial)
                    {
                        _navMeshAgent.SetDestination(hit.position);
                        _navMeshAgent.isStopped = false;
                        return true;
                    }
                }
            }

            // 如果计算路径失败，直接尝试敌人位置
            return _navMeshAgent.SetDestination(enemyPos);
        }

        private void AttemptUnstuck()
        {
            // 尝试向随机方向移动来摆脱卡住状态
            for (int i = 0; i < 8; i++)
            {
                Vector3 randomDirection = Random.insideUnitSphere;
                randomDirection.y = 0;
                randomDirection = randomDirection.normalized * unstuckMoveDistance;

                Vector3 targetPos = transform.position + randomDirection;

                if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, unstuckMoveDistance * 2f, NavMesh.AllAreas))
                {
                    NavMeshPath path = new NavMeshPath();
                    if (_navMeshAgent.CalculatePath(hit.position, path) &&
                        path.status != NavMeshPathStatus.PathInvalid)
                    {
                        _navMeshAgent.SetDestination(hit.position);
                        _navMeshAgent.isStopped = false;
                        return;
                    }
                }
            }
        }
    }
}
