using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using UnityEngine.AI;
using System.Collections.Generic;

namespace Assets.Scripts.CombatLogic.Characters.BehaviorTreeExtend
{
    /// <summary>
    /// 到达目的地和被攻击时都会sucess
    /// 参考这个 <see cref="BehaviorDesigner.Runtime.Tasks.Movement.Cover"/>
    /// 参考这个 <see cref="BehaviorDesigner.Runtime.Tasks"/>
    /// </summary>
    [TaskCategory("Tactical")]
    [TaskDescription("written in behavior tree extend")]
    [TaskName("WindBack")]
    internal class WindBackTask : Action
    {
        public SharedGameObjectList TargetGroup;
        private NavMeshAgent agent;

        Queue<Vector3> dests;
        public override void OnStart()
        {
            agent = GetComponent<NavMeshAgent>();

            Vector3 pivot = TargetGroup.Value[0].transform.position; // 旋转中心点坐标
            Vector3 point = transform.position; // 要旋转的点坐标

            // 计算旋转后的位置
            Vector3 rotatedPoint1 = Quaternion.Euler(0f, 90f, 0f) * (point - pivot).normalized * 10 + pivot;
            Vector3 rotatedPoint2 = Quaternion.Euler(0f, -90f, 0f) * (point - pivot).normalized * 10 + pivot;

            void generateDests(float angle)
            {
                dests = new Queue<Vector3>();
                for(int i = 0; i < 10; i++)
                {
                    dests.Enqueue(Quaternion.Euler(0f, angle / 10 * i, 0f) * (point - pivot).normalized * 10 + pivot);
                }
            }

            // 选择距离短的
            if(Vector3.Distance(rotatedPoint1, point) < Vector3.Distance(rotatedPoint2, point)) generateDests(90f);
            else generateDests(-90f);
            agent.SetDestination(dests.Peek());
        }

        public override TaskStatus OnUpdate()
        {
            // todo：被攻击时返回sucess终止行为

            if (dests.Count != 0 && Vector3.Distance(transform.position, dests.Peek()) < 1f) {
                if (dests.Count == 0) return TaskStatus.Success;
                else
                {
                    dests.Dequeue();
                    agent.SetDestination(dests.Peek());
                    return TaskStatus.Running;
                }
            } 
            else return TaskStatus.Running;
        }


    }
}
