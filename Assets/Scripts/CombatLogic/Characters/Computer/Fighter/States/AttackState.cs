using Assets.Scripts.CombatLogic.ContextExtends;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace Assets.Scripts.CombatLogic.Characters.Computer.Fighter.States
{
    internal class AttackState : IFighterState
    {
        FighterController _controller;
        float min_mag;
        private CombatContextManager _context => CombatContextManager.Instance;
        public AttackState(FighterController controller)
        {
            _controller = controller;
        }
        public void OnEnter()
        {
            if (_controller.Aim == null || _context.Operators[_controller.Aim].IsDead) 
                _controller.Aim = _context.GetNealyCounter(_controller.transform, _controller.Team);
            _controller.m_NavAgent.autoBraking = false;
            min_mag = 100;
        }

        public void OnExit()
        {
            _controller.m_NavAgent.autoBraking = true;
        }

        public void OnUpdate()
        {
            if (_controller.Aim == null)
            {
                _controller.TranslateState(IFighterState.StateType.Idle);
                return;
            }
            else if (_context.Operators[_controller.Aim].IsDead)
            {
                _controller.Aim = _context.GetNealyCounter(_controller.transform, _controller.Team);
                if (_controller.Aim == null)
                {
                    _controller.TranslateState(IFighterState.StateType.Idle);
                    return;
                }
            }
            // 在过程中投炸弹
            _controller.SetDest(_controller.Aim.position + (_controller.Aim.position - _controller.transform.position).normalized * 4);
            var distance = _controller.Aim.position - _controller.transform.position;
            distance.y = 0;
            if (distance.magnitude < 4 && distance.magnitude > min_mag)
            {
                // 以使用次数代替冷却cd的技能，回到cvbase后补充的技能
                _controller.DiveBombing(_controller.Aim.position);
                _controller.TranslateState(IFighterState.StateType.Return);
            }
            min_mag = Mathf.Min(min_mag, distance.magnitude);
        }
    }
}
