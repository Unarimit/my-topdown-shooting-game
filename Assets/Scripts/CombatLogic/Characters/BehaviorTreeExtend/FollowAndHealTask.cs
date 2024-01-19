using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tactical.Tasks;
using System;

namespace Assets.Scripts.CombatLogic.Characters.BehaviorTreeExtend
{
    [TaskName("FollowAndHeal")]
    [TaskCategory("Tactical")]
    [TaskDescription("written in behavior tree extend")]
    internal class FollowAndHealTask: NavMeshTacticalGroup
    {
        /// <summary> 保持的距离 </summary>
        float offset = 2.0f;
        public override TaskStatus OnUpdate() {
            var baseStatus = base.OnUpdate();

            if(targetGroup.Value.Count != 1)
            {
                throw new Exception($"targetGroup should be 1 element in FollowAndHealTask, but is {targetGroup.Value.Count}");
            }


            // follow
            var targetPos = targetGroup.Value[0].transform.position;
            var myPos = transform.position;
            var dest = targetPos - (targetPos - myPos).normalized * offset;
            if (Vector3.Distance(targetPos, myPos) > offset * 1.5f) tacticalAgent.SetDestination(dest);

            // heal
            if (targetGroup.Value[0].GetComponent<OperatorBehaviorTreeController>().IsHurt() is true)
            {
                if (MoveToAttackPosition())
                {
                    tacticalAgent.TryAttack();
                }
            }

            return TaskStatus.Running;
        }
    }
}
