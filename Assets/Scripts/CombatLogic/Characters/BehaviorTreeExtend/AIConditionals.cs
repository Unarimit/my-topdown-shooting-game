using Assets.Scripts.CombatLogic.Characters.Computer.Agent;
using Assets.Scripts.Entities;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.Characters.BehaviorTreeExtend
{
    /// <summary>
    /// 检查视野内是否有敌人
    /// </summary>
    [TaskCategory("AI/Conditions")]
    [TaskDescription("Returns Success if there is an enemy in sight")]
    internal class HasEnemyInSight : Conditional
    {
        private AgentController _agentController;

        public override void OnStart()
        {
            _agentController = GetComponent<AgentController>();
        }

        public override TaskStatus OnUpdate()
        {
            if (_agentController == null) return TaskStatus.Failure;

            bool hasEnemy = _agentController.HasEnemyInRange();
            return hasEnemy ? TaskStatus.Success : TaskStatus.Failure;
        }
    }

    /// <summary>
    /// 检查是否低血量
    /// </summary>
    [TaskCategory("AI/Conditions")]
    [TaskDescription("Returns Success if health is low (below 1/3)")]
    internal class IsLowHealth : Conditional
    {
        private AgentController _agentController;

        public override void OnStart()
        {
            _agentController = GetComponent<AgentController>();
        }

        public override TaskStatus OnUpdate()
        {
            if (_agentController == null) return TaskStatus.Failure;

            bool isLowHealth = _agentController.IsLowHealth();
            return isLowHealth ? TaskStatus.Success : TaskStatus.Failure;
        }
    }

    /// <summary>
    /// 检查是否没有弹药
    /// </summary>
    [TaskCategory("AI/Conditions")]
    [TaskDescription("Returns Success if out of ammo")]
    internal class IsOutOfAmmo : Conditional
    {
        private AgentController _agentController;

        public override void OnStart()
        {
            _agentController = GetComponent<AgentController>();
        }

        public override TaskStatus OnUpdate()
        {
            if (_agentController == null) return TaskStatus.Failure;

            bool hasAmmo = _agentController.HasAmmo();
            return !hasAmmo ? TaskStatus.Success : TaskStatus.Failure;
        }
    }

    /// <summary>
    /// 检查是否有受伤的队友需要治疗
    /// </summary>
    [TaskCategory("AI/Conditions")]
    [TaskDescription("Returns Success if there is a hurt teammate nearby")]
    internal class HasHurtTeammate : Conditional
    {
        private AgentController _agentController;

        public override void OnStart()
        {
            _agentController = GetComponent<AgentController>();
        }

        public override TaskStatus OnUpdate()
        {
            if (_agentController == null) return TaskStatus.Failure;

            GameObject teammate = _agentController.GetNearestTeammate();
            if (teammate == null) return TaskStatus.Failure;

            // 检查队友是否受伤
            var teammateController = teammate.GetComponent<OperatorBehaviorTreeController>();
            if (teammateController != null && teammateController.IsHurt())
            {
                // 设置teammate变量供后续任务使用
                var teammateVar = Owner.GetVariable("teammate") as SharedGameObjectList;
                if (teammateVar != null)
                {
                    teammateVar.Value = new System.Collections.Generic.List<GameObject> { teammate };
                }
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    }

    /// <summary>
    /// 设置目标敌人为变量
    /// </summary>
    [TaskCategory("AI/Actions")]
    [TaskDescription("Sets the nearest enemy as target variable")]
    internal class SetEnemyTarget : Action
    {
        private AgentController _agentController;

        public override void OnStart()
        {
            _agentController = GetComponent<AgentController>();
        }

        public override TaskStatus OnUpdate()
        {
            if (_agentController == null) return TaskStatus.Failure;

            GameObject enemy = _agentController.GetNearestEnemy();
            if (enemy == null) return TaskStatus.Failure;

            _agentController.SetTarget(enemy);
            return TaskStatus.Success;
        }
    }

    /// <summary>
    /// 设置移动目标
    /// </summary>
    [TaskCategory("AI/Actions")]
    [TaskDescription("Sets a patrol position as move target")]
    internal class SetPatrolTarget : Action
    {
        private AgentController _agentController;

        public override void OnStart()
        {
            _agentController = GetComponent<AgentController>();
        }

        public override TaskStatus OnUpdate()
        {
            if (_agentController == null) return TaskStatus.Failure;

            Vector3 patrolPos = _agentController.GetPatrolPosition();
            _agentController.SetMoveTarget(patrolPos);
            return TaskStatus.Success;
        }
    }

    /// <summary>
    /// 检查是否是进攻型角色
    /// </summary>
    [TaskCategory("AI/Conditions")]
    [TaskDescription("Returns Success if operator has Offensive trait")]
    internal class IsOffensiveTrait : Conditional
    {
        private AgentController _agentController;

        public override void OnStart()
        {
            _agentController = GetComponent<AgentController>();
        }

        public override TaskStatus OnUpdate()
        {
            if (_agentController == null) return TaskStatus.Failure;

            return _agentController.GetTrait() == OperatorTrait.Offensive 
                ? TaskStatus.Success 
                : TaskStatus.Failure;
        }
    }

    /// <summary>
    /// 检查是否是谨慎型角色
    /// </summary>
    [TaskCategory("AI/Conditions")]
    [TaskDescription("Returns Success if operator has Timid trait")]
    internal class IsTimidTrait : Conditional
    {
        private AgentController _agentController;

        public override void OnStart()
        {
            _agentController = GetComponent<AgentController>();
        }

        public override TaskStatus OnUpdate()
        {
            if (_agentController == null) return TaskStatus.Failure;

            return _agentController.GetTrait() == OperatorTrait.Timid 
                ? TaskStatus.Success 
                : TaskStatus.Failure;
        }
    }
}