using UnityEngine;

namespace Assets.Scripts.CombatLogic.Characters.Computer.Fighter.States
{
    internal class IdleState : IFighterState
    {
        FighterController _controller;
        public IdleState(FighterController controller)
        {
            _controller = controller;
        }
        float idleTime = 1;
        public void OnEnter()
        {
            idleTime = 1;
        }

        public void OnExit()
        {

        }

        public void OnUpdate()
        {
            _controller.SetDest(_controller.CvBase.position);
            idleTime -= Time.deltaTime;
            if (idleTime < 0) _controller.TranslateState(IFighterState.StateType.Attack);
        }
    }
}
