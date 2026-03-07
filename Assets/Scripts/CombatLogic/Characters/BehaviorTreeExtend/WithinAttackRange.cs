using Assets.Scripts.CombatLogic.Characters.Computer.Agent;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.Characters.BehaviorTreeExtend
{
    /// <summary>
    /// 检查敌人在攻击范围内
    /// </summary>
    [TaskCategory("AI/Conditions")]
    [TaskDescription("Returns Success if nearest enemy is within attack range")]
    internal class WithinAttackRange : Conditional
    {
        private AgentController _agentController;

        public override void OnStart()
        {
            _agentController = GetComponent<AgentController>();
        }

        public override TaskStatus OnUpdate()
        {
            if (_agentController == null) return TaskStatus.Failure;

            var enemy = _agentController.GetNearestEnemy();
            if (enemy == null) return TaskStatus.Failure;

            float attackRange = _agentController.Model.AttackRange;
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            
            return distance <= attackRange ? TaskStatus.Success : TaskStatus.Failure;
        }
    }

    /// <summary>
    /// 向敌人移动
    /// </summary>
    [TaskCategory("AI/Actions")]
    [TaskDescription("Move towards the nearest enemy")]
    internal class MoveToEnemyTask : Action
    {
        private AgentController _agentController;
        private UnityEngine.AI.NavMeshAgent _navMeshAgent;

        public override void OnStart()
        {
            _agentController = GetComponent<AgentController>();
            _navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        }

        public override TaskStatus OnUpdate()
        {
            if (_agentController == null || _navMeshAgent == null)
                return TaskStatus.Failure;

            var enemy = _agentController.GetNearestEnemy();
            if (enemy == null|| CombatContextManager.Instance.Operators[enemy.transform].IsDead)
                return TaskStatus.Failure;

            if(_navMeshAgent.SetDestination(enemy.transform.position) is false)
                return TaskStatus.Failure;
            
            float attackRange = _agentController.Model.AttackRange;
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            
            if (distance <= attackRange)
            {
                return TaskStatus.Success;
            }
            
            return TaskStatus.Running;
        }
    }
}