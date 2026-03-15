using Assets.Scripts.CombatLogic.Characters.Computer.Agent;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.CombatLogic.Characters.BehaviorTreeExtend
{
    /// <summary>
    /// 简单巡逻任务 - 作为默认fallback使用
    /// 解决Agent发呆问题，确保Agent总有移动目标
    /// </summary>
    [TaskCategory("AI/Actions")]
    [TaskDescription("Simple patrol task that moves to a random position. Used as default fallback.")]
    internal class SimplePatrolTask : Action
    {
        private AgentController _agentController;
        private NavMeshAgent _navMeshAgent;

        [UnityEngine.Tooltip("巡逻半径")]
        public float patrolRadius = 10f;

        [UnityEngine.Tooltip("最小移动距离")]
        public float minMoveDistance = 3f;

        [UnityEngine.Tooltip("到达目标后的等待时间")]
        public float waitTime = 1f;

        private Vector3 _targetPosition;
        private float _waitTimer;
        private bool _isWaiting;
        private float _startTime;
        private const float MAX_EXECUTION_TIME = 10f;

        public override void OnStart()
        {
            _agentController = GetComponent<AgentController>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _startTime = Time.time;
            _isWaiting = false;
            _waitTimer = 0f;

            // 立即设置目标
            SetRandomDestination();
        }

        public override TaskStatus OnUpdate()
        {
            if (_agentController == null || _navMeshAgent == null)
                return TaskStatus.Failure;

            // 超时保护
            if (Time.time - _startTime > MAX_EXECUTION_TIME)
            {
                return TaskStatus.Success;
            }

            // 等待状态
            if (_isWaiting)
            {
                _waitTimer += Time.deltaTime;
                if (_waitTimer >= waitTime)
                {
                    return TaskStatus.Success; // 完成一轮巡逻
                }
                return TaskStatus.Running;
            }

            // 检查是否到达目的地
            if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance < 1.0f)
            {
                _isWaiting = true;
                _waitTimer = 0f;
                return TaskStatus.Running;
            }

            // 检查是否卡住
            if (_navMeshAgent.isPathStale || (!_navMeshAgent.hasPath && !_navMeshAgent.pathPending))
            {
                // 尝试新目标
                SetRandomDestination();
            }

            return TaskStatus.Running;
        }

        private void SetRandomDestination()
        {
            for (int i = 0; i < 15; i++)
            {
                Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
                randomDirection.y = 0;

                if (randomDirection.magnitude < minMoveDistance)
                    randomDirection = randomDirection.normalized * minMoveDistance;

                Vector3 targetPos = transform.position + randomDirection;

                // 确保在NavMesh上
                if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
                {
                    // 验证路径可达
                    NavMeshPath path = new NavMeshPath();
                    if (_navMeshAgent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete)
                    {
                        _targetPosition = hit.position;
                        _navMeshAgent.SetDestination(_targetPosition);
                        _navMeshAgent.isStopped = false;
                        return;
                    }
                }
            }
        }

        public override void OnEnd()
        {
            if (_navMeshAgent != null)
            {
                _navMeshAgent.isStopped = false;
            }
        }
    }
}
