using Assets.Scripts.CombatLogic.Characters.Computer.Agent;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.Characters.BehaviorTreeExtend
{
    /// <summary>
    /// 检查Agent是否长时间没有检测到敌人
    /// 用于触发扩大搜索范围或改变巡逻策略
    /// </summary>
    [TaskCategory("AI/Conditions")]
    [TaskDescription("Returns Success if agent hasn't seen enemy for specified time")]
    internal class NoEnemyForLongTime : Conditional
    {
        private AgentController _agentController;

        [UnityEngine.Tooltip("多长时间没有看到敌人触发(秒)")]
        public float noEnemyThreshold = 10f;

        [UnityEngine.Tooltip("是否重置计时器当此条件成功")]
        public bool resetOnSuccess = true;

        private float _noEnemyTimer;
        private bool _wasReset;

        public override void OnStart()
        {
            _agentController = GetComponent<AgentController>();
            _wasReset = false;

            // 如果之前被重置，继续计时
            if (!_wasReset)
            {
                // 保持现有计时器值
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (_agentController == null)
                return TaskStatus.Failure;

            // 检查当前是否有敌人在视野中
            bool hasEnemy = _agentController.HasEnemyInRange();

            if (hasEnemy)
            {
                // 看到敌人，重置计时器
                _noEnemyTimer = 0f;
                _wasReset = false;
                return TaskStatus.Failure;
            }

            // 没有看到敌人，累加计时器
            _noEnemyTimer += Time.deltaTime;

            if (_noEnemyTimer >= noEnemyThreshold)
            {
                if (resetOnSuccess)
                {
                    _noEnemyTimer = 0f;
                    _wasReset = true;
                }
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }

        public override void OnEnd()
        {
            // 条件结束时保持计时器值
        }
    }

    /// <summary>
    /// 扩大搜索巡逻任务 - 使用更大的巡逻半径
    /// </summary>
    [TaskCategory("AI/Actions")]
    [TaskDescription("Patrol with expanded search radius")]
    internal class ExpandedSearchTask : Action
    {
        private AgentController _agentController;
        private UnityEngine.AI.NavMeshAgent _navMeshAgent;

        [UnityEngine.Tooltip("扩大后的搜索半径")]
        public float expandedRadius = 20f;

        [UnityEngine.Tooltip("向敌人可能位置移动")]
        public bool moveTowardEnemySpawn = true;

        private float _startTime;
        private const float EXECUTION_TIME = 8f;

        public override void OnStart()
        {
            _agentController = GetComponent<AgentController>();
            _navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            _startTime = Time.time;

            SetExpandedSearchDestination();
        }

        public override TaskStatus OnUpdate()
        {
            if (_agentController == null || _navMeshAgent == null)
                return TaskStatus.Failure;

            // 执行时间限制
            if (Time.time - _startTime > EXECUTION_TIME)
            {
                return TaskStatus.Success;
            }

            // 检查是否看到敌人
            if (_agentController.HasEnemyInRange())
            {
                return TaskStatus.Success; // 成功发现敌人
            }

            // 检查是否到达目的地
            if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance < 1.5f)
            {
                // 到达后继续移动到新位置
                SetExpandedSearchDestination();
            }

            // 检查路径状态
            if (_navMeshAgent.isPathStale || (!_navMeshAgent.hasPath && !_navMeshAgent.pathPending))
            {
                SetExpandedSearchDestination();
            }

            return TaskStatus.Running;
        }

        private void SetExpandedSearchDestination()
        {
            Vector3 targetPos;

            if (moveTowardEnemySpawn)
            {
                // 向地图中心或敌人可能的出生点移动
                // 简化处理：向当前位置的对面移动
                targetPos = transform.position + (transform.forward * expandedRadius * 0.5f);
            }
            else
            {
                // 随机位置
                Vector3 randomDir = Random.insideUnitSphere;
                randomDir.y = 0;
                targetPos = transform.position + randomDir.normalized * expandedRadius;
            }

            // 确保在NavMesh上
            if (UnityEngine.AI.NavMesh.SamplePosition(targetPos, out UnityEngine.AI.NavMeshHit hit, expandedRadius, UnityEngine.AI.NavMesh.AllAreas))
            {
                _navMeshAgent.SetDestination(hit.position);
                _navMeshAgent.isStopped = false;
            }
        }
    }
}
