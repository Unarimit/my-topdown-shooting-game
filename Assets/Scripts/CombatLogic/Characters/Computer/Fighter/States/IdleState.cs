using Assets.Scripts.CombatLogic.ContextExtends;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.Characters.Computer.Fighter.States
{
    internal class IdleState : IFighterState
    {
        FighterController _controller;
        CombatContextManager _context => CombatContextManager.Instance;
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
            if (idleTime < 0) {

                if (_context.CanReact(_controller.Team)) _controller.TranslateState(IFighterState.StateType.Attack);
                else
                {
                    var aim = _context.GetNealyCounter(_controller.transform, _controller.Team);
                    if(aim != null && (aim.position - _controller.transform.position).sqrMagnitude < 16)
                    {
                        _controller.TranslateState(IFighterState.StateType.Attack);
                    }
                }
            } 
        }
    }
}
