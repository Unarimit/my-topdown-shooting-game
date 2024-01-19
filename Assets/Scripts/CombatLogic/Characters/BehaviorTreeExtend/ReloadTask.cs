using BehaviorDesigner.Runtime.Tactical.Tasks;
using BehaviorDesigner.Runtime.Tasks;

namespace Assets.Scripts.CombatLogic.Characters.BehaviorTreeExtend
{
    [TaskCategory("Tactical")]
    [TaskDescription("written in behavior tree extend")]
    [TaskName("Reload")]
    internal class ReloadTask : Action
    {
        public override TaskStatus OnUpdate()
        {
            if (GetComponent<OperatorController>() == null) throw new System.Exception("can not find `OperatorController` component");

            GetComponent<OperatorController>().Reload();
            return TaskStatus.Success;
        }
    }
}
