using BehaviorDesigner.Runtime.Tactical.Tasks;
using BehaviorDesigner.Runtime.Tasks;

namespace Assets.Scripts.CombatLogic.Characters.BehaviorTreeExtend
{
    /// <summary>
    /// 取消所有影响逃跑的动作
    /// </summary>
    [TaskCategory("Tactical")]
    [TaskDescription("written in behavior tree extend")]
    [TaskName("EnterRunMode")]
    internal class EnterRunMode :Action
    {
        public override TaskStatus OnUpdate()
        {
            if (GetComponent<OperatorController>() == null) throw new System.Exception("can not find `OperatorController` component");

            GetComponent<OperatorController>().ClearAnimate();
            return TaskStatus.Success;
        }
    }
}
