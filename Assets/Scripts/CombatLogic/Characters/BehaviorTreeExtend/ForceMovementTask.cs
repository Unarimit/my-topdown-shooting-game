using Assets.Scripts.CombatLogic.Characters.Computer.Agent;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.CombatLogic.Characters.BehaviorTreeExtend
{
    /// <summary>
    /// 强制移动任务 - 解决Agent划水问题
    /// 当Agent静止太久时，强制向随机方向移动
    /// </summary>
    [TaskCategory("AI/Actions")]
    [TaskDescription("Forces agent to move if stationary for too long")]
    internal class ForceMovementTask : Action
    {
        private AgentController _agentController;
        private NavMeshAgent _navMeshAgent;
        
        // 移动参数
        public float moveDistance = 8f;
        public float minMoveDistance = 3f;
        
        // 内部状态
        private Vector3 _targetPosition;
        private bool _hasValidDestination;
        private float _startTime;
        private float _maxExecutionTime = 5f;

        public override void OnStart()
        {
            _agentController = GetComponent<AgentController>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _startTime = Time.time;
            
            // 立即设置一个随机目的地
            SetRandomDestination();
        }

        public override TaskStatus OnUpdate()
        {
            if (_agentController == null || _navMeshAgent == null)
                return TaskStatus.Failure;

            // 超时检查
            if (Time.time - _startTime > _maxExecutionTime)
            {
                return TaskStatus.Success; // 强制结束，让行为树继续
            }

            // 如果没有有效目的地，尝试设置一个
            if (!_hasValidDestination)
            {
                SetRandomDestination();
                if (!_hasValidDestination)
                    return TaskStatus.Failure;
            }

            // 检查是否到达目的地
            if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance < 1.5f)
            {
                return TaskStatus.Success;
            }

            // 检查是否卡住（路径无效或无法移动）
            if (_navMeshAgent.isPathStale || (!_navMeshAgent.hasPath && !_navMeshAgent.pathPending))
            {
                // 尝试新的目的地
                SetRandomDestination();
            }

            return TaskStatus.Running;
        }

        private void SetRandomDestination()
        {
            _hasValidDestination = false;
            
            // 尝试多个随机方向
            for (int i = 0; i < 10; i++)
            {
                // 生成随机方向（360度）
                Vector3 randomDirection = new Vector3(
                    Random.Range(-1f, 1f), 
                    0, 
                    Random.Range(-1f, 1f)
                ).normalized;
                
                if (randomDirection == Vector3.zero)
                    randomDirection = transform.forward;
                
                // 随机距离
                float distance = Random.Range(minMoveDistance, moveDistance);
                Vector3 targetPos = transform.position + randomDirection * distance;
                
                // 在NavMesh上寻找有效位置
                if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 5f, NavMesh.AllAreas))
                {
                    // 确保位置可达
                    NavMeshPath path = new NavMeshPath();
                    if (_navMeshAgent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete)
                    {
                        _targetPosition = hit.position;
                        _navMeshAgent.SetDestination(_targetPosition);
                        _hasValidDestination = true;
                        
                        // 确保NavMeshAgent是启用的
                        _navMeshAgent.isStopped = false;
                        
                        return;
                    }
                }
            }
        }

        public override void OnEnd()
        {
            // 清理状态
            _hasValidDestination = false;
        }
    }

    /// <summary>
    /// 检查Agent是否静止太久（用于触发强制移动）
    /// </summary>
    [TaskCategory("AI/Conditions")]
    [TaskDescription("Returns Success if agent has been stationary for too long")]
    internal class IsStationaryTooLong : Conditional
    {
        private AgentController _agentController;
        private NavMeshAgent _navMeshAgent;
        
        public float stationaryThreshold = 3f; // 静止超过3秒触发
        
        private Vector3 _lastPosition;
        private float _stationaryTimer;

        public override void OnStart()
        {
            _agentController = GetComponent<AgentController>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _lastPosition = transform.position;
            _stationaryTimer = 0f;
        }

        public override TaskStatus OnUpdate()
        {
            if (_agentController == null) return TaskStatus.Failure;

            // 检查位置变化
            float movedDistance = Vector3.Distance(transform.position, _lastPosition);
            if (movedDistance < 0.1f)
            {
                _stationaryTimer += Time.deltaTime;
            }
            else
            {
                _stationaryTimer = 0f;
            }
            _lastPosition = transform.position;

            // 检查NavMeshAgent速度
            if (_navMeshAgent != null && _navMeshAgent.velocity.magnitude > 0.5f)
            {
                _stationaryTimer = 0f; // 正在移动，重置计时器
            }

            return _stationaryTimer >= stationaryThreshold ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}