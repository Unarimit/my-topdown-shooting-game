using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.CombatLogic.Characters.BehaviorTreeExtend
{
    [TaskCategory("Tactical")]
    [TaskDescription("written in behavior tree extend")]
    [TaskName("NavMeshRandomWalk")]
    internal class NavMeshRandomWalkTask : Action
    {
        public override TaskStatus OnUpdate()
        {
            if (GetComponent<NavMeshAgent>() == null) throw new System.Exception("can not find `NavMeshAgent` component");

            GetComponent<NavMeshAgent>().SetDestination(transform.position + new Vector3(Random.Range(3f, 5f), 0, Random.Range(3f, 5f)));
            return TaskStatus.Success;
        }
    }
}
