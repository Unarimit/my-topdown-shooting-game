using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using UnityEngine.AI;

namespace Assets.Scripts.CombatLogic.Characters.BehaviorTreeExtend
{
    
    [TaskName("FollowAndHeal")]
    [TaskCategory("Tactical")]
    [TaskDescription("written in behavior tree extend")]
    internal class MoveForwardTask : Action
    {
        public SharedVector3 MovePos;
        NavMeshAgent agent;
        public override void OnStart()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.SetDestination(MovePos.Value);
        }

        public override TaskStatus OnUpdate()
        {
            agent.isStopped = false;
            if (Vector3.Distance(transform.position, agent.destination) < 1f) return TaskStatus.Success;
            else return TaskStatus.Running;
        }
    }
}
