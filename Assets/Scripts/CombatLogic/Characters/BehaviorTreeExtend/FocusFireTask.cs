using Assets.Scripts.CombatLogic.Characters.Computer.Agent;
using Assets.Scripts.CombatLogic.ContextExtends;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.Characters.BehaviorTreeExtend
{
    /// <summary>
    /// 寻找被友方集中攻击的目标（集火逻辑）
    /// </summary>
    [TaskCategory("AI/Actions")]
    [TaskDescription("Finds enemy that teammates are attacking for focus fire")]
    internal class FindFocusedTarget : Action
    {
        private AgentController _agentController;
        private CombatContextManager _context;

        [UnityEngine.Tooltip("检测范围")]
        public float detectionRange = 30f;

        public override void OnStart()
        {
            _agentController = GetComponent<AgentController>();
            _context = CombatContextManager.Instance;
        }

        public override TaskStatus OnUpdate()
        {
            if (_agentController == null || _context == null)
                return TaskStatus.Failure;

            // 获取当前目标
            var currentTarget = _agentController.GetNearestEnemy();
            if (currentTarget == null) return TaskStatus.Failure;

            // 寻找被最多友方攻击的敌人
            GameObject focusedTarget = FindTargetWithMostAttackers();
            
            if (focusedTarget != null && focusedTarget != currentTarget)
            {
                // 切换到集火目标
                _agentController.SetTarget(focusedTarget);
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }

        private GameObject FindTargetWithMostAttackers()
        {
            int myTeam = _agentController.Model.Team;
            Dictionary<GameObject, int> targetAttackCount = new Dictionary<GameObject, int>();

            // 统计每个敌人被多少友方攻击
            foreach (var op in _context.Operators)
            {
                if (op.Value.Team != myTeam) continue; // 只统计友方
                if (op.Key == transform) continue; // 排除自己
                if (op.Value.IsDead) continue;

                // 获取该友方的目标
                var allyAgent = op.Key.GetComponent<AgentController>();
                if (allyAgent != null)
                {
                    var allyTarget = allyAgent.GetNearestEnemy();
                    if (allyTarget != null)
                    {
                        if (!targetAttackCount.ContainsKey(allyTarget))
                            targetAttackCount[allyTarget] = 0;
                        targetAttackCount[allyTarget]++;
                    }
                }
            }

            // 找到被攻击最多的目标
            if (targetAttackCount.Count > 0)
            {
                var maxPair = targetAttackCount.OrderByDescending(p => p.Value).First();
                if (maxPair.Value >= 2) // 至少有2个队友在攻击
                {
                    return maxPair.Key;
                }
            }

            return null;
        }
    }

    /// <summary>
    /// 检查是否有集火机会（有队友正在攻击的敌人）
    /// </summary>
    [TaskCategory("AI/Conditions")]
    [TaskDescription("Returns Success if there's a focus fire opportunity")]
    internal class HasFocusFireOpportunity : Conditional
    {
        private AgentController _agentController;
        private CombatContextManager _context;

        public override void OnStart()
        {
            _agentController = GetComponent<AgentController>();
            _context = CombatContextManager.Instance;
        }

        public override TaskStatus OnUpdate()
        {
            if (_agentController == null || _context == null)
                return TaskStatus.Failure;

            int myTeam = _agentController.Model.Team;
            int attackersCount = 0;
            GameObject potentialTarget = null;

            foreach (var op in _context.Operators)
            {
                if (op.Value.Team != myTeam) continue;
                if (op.Key == transform) continue;
                if (op.Value.IsDead) continue;

                var allyAgent = op.Key.GetComponent<AgentController>();
                if (allyAgent != null)
                {
                    var allyTarget = allyAgent.GetNearestEnemy();
                    if (allyTarget != null)
                    {
                        attackersCount++;
                        potentialTarget = allyTarget;
                    }
                }
            }

            return (attackersCount >= 2 && potentialTarget != null) 
                ? TaskStatus.Success 
                : TaskStatus.Failure;
        }
    }
}