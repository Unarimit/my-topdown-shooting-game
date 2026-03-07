using Assets.Scripts.CombatLogic.Characters.Computer.Agent;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.CombatLogic.Characters.BehaviorTreeExtend
{
    /// <summary>
    /// 寻找掩体并移动过去
    /// </summary>
    [TaskCategory("AI/Actions")]
    [TaskDescription("Finds cover position away from enemies and moves there")]
    internal class FindCoverTask : Action
    {
        private AgentController _agentController;
        private NavMeshAgent _navMeshAgent;
        
        [UnityEngine.Tooltip("撤退距离")]
        public float retreatDistance = 10f;
        
        [UnityEngine.Tooltip("最大尝试次数")]
        public int maxAttempts = 5;

        public override void OnStart()
        {
            _agentController = GetComponent<AgentController>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        public override TaskStatus OnUpdate()
        {
            if (_agentController == null || _navMeshAgent == null)
                return TaskStatus.Failure;

            // 寻找远离敌人的位置
            Vector3 coverPosition = FindCoverPosition();
            if (coverPosition != Vector3.zero)
            {
                _navMeshAgent.SetDestination(coverPosition);
                
                // 检查是否到达目标点
                if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance < 1f)
                {
                    return TaskStatus.Success;
                }
                
                return TaskStatus.Running;
            }

            return TaskStatus.Failure;
        }

        private Vector3 FindCoverPosition()
        {
            Vector3 bestPosition = Vector3.zero;
            float bestScore = float.MinValue;
            
            // 获取最近敌人的方向
            Vector3 awayFromEnemy = Vector3.zero;
            var nearestEnemy = _agentController.GetNearestEnemy();
            if (nearestEnemy != null)
            {
                awayFromEnemy = (transform.position - nearestEnemy.transform.position).normalized;
            }

            for (int i = 0; i < maxAttempts; i++)
            {
                // 在撤退方向随机选择点
                Vector3 randomDirection = Random.insideUnitSphere;
                randomDirection.y = 0;
                
                if (awayFromEnemy != Vector3.zero)
                {
                    // 偏向远离敌人的方向
                    randomDirection = (awayFromEnemy + randomDirection * 0.5f).normalized;
                }
                
                Vector3 testPosition = transform.position + randomDirection * retreatDistance;
                
                // 检查是否在NavMesh上
                if (NavMesh.SamplePosition(testPosition, out NavMeshHit hit, 5f, NavMesh.AllAreas))
                {
                    float score = CalculatePositionScore(hit.position);
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestPosition = hit.position;
                    }
                }
            }

            return bestPosition;
        }

        private float CalculatePositionScore(Vector3 position)
        {
            float score = 0f;
            
            // 距离最近敌人的距离（越远越好）
            var nearestEnemy = _agentController.GetNearestEnemy();
            if (nearestEnemy != null)
            {
                float distToEnemy = Vector3.Distance(position, nearestEnemy.transform.position);
                score += distToEnemy * 2f;
            }
            
            // 距离当前位置的距离（不要太远，避免绕路）
            float distFromCurrent = Vector3.Distance(position, transform.position);
            score -= distFromCurrent * 0.5f;
            
            // 随机因子（增加多样性）
            score += Random.Range(0f, 5f);
            
            return score;
        }
    }
}