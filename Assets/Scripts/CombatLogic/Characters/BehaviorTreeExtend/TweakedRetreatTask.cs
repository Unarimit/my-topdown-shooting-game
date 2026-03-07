using Assets.Scripts.CombatLogic.Characters.Computer.Agent;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.CombatLogic.Characters.BehaviorTreeExtend
{
    /// <summary>
    /// 优化版寻找掩体 - 增加撤退距离
    /// </summary>
    [TaskCategory("AI/Actions")]
    [TaskDescription("Enhanced cover finding with longer retreat distance")]
    internal class TweakedRetreatTask : Action
    {
        private AgentController _agentController;
        private NavMeshAgent _navMeshAgent;
        
        [UnityEngine.Tooltip("优化后的撤退距离")]
        public float retreatDistance = 12f; // 从10增加到12
        
        [UnityEngine.Tooltip("最大尝试次数")]
        public int maxAttempts = 8; // 从5增加到8

        public override void OnStart()
        {
            _agentController = GetComponent<AgentController>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        public override TaskStatus OnUpdate()
        {
            if (_agentController == null || _navMeshAgent == null)
                return TaskStatus.Failure;

            Vector3 coverPosition = FindCoverPosition();
            if (coverPosition != Vector3.zero)
            {
                _navMeshAgent.SetDestination(coverPosition);
                
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
            
            Vector3 awayFromEnemy = Vector3.zero;
            var nearestEnemy = _agentController.GetNearestEnemy();
            if (nearestEnemy != null)
            {
                awayFromEnemy = (transform.position - nearestEnemy.transform.position).normalized;
            }

            for (int i = 0; i < maxAttempts; i++)
            {
                Vector3 randomDirection = Random.insideUnitSphere;
                randomDirection.y = 0;
                
                if (awayFromEnemy != Vector3.zero)
                {
                    randomDirection = (awayFromEnemy + randomDirection * 0.3f).normalized; // 更偏向远离敌人
                }
                
                Vector3 testPosition = transform.position + randomDirection * retreatDistance;
                
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
            
            var nearestEnemy = _agentController.GetNearestEnemy();
            if (nearestEnemy != null)
            {
                float distToEnemy = Vector3.Distance(position, nearestEnemy.transform.position);
                score += distToEnemy * 3f; // 增加敌人距离权重
            }
            
            float distFromCurrent = Vector3.Distance(position, transform.position);
            score -= distFromCurrent * 0.3f; // 减少移动距离惩罚
            
            score += Random.Range(0f, 3f);
            
            return score;
        }
    }
}