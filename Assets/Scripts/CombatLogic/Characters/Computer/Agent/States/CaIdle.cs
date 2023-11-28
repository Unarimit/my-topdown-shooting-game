using Assets.Scripts.CombatLogic.ContextExtends;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.Characters.Computer.Agent.States
{
    public class CaIdle : IAgentState
    {
        private AgentController _agent;
        private CombatContextManager _context = CombatContextManager.Instance;
        public CaIdle(AgentController agent)
        {
            _agent = agent;
        }
        public void OnEnter()
        {
            // do nothing
        }

        public void OnExit()
        {
            // do nothing
        }

        private float _idleDelta = 4;
        private float _translateDelta = 1;
        public void OnUpdate()
        {
            _idleDelta += Time.deltaTime;
            _translateDelta += Time.deltaTime;

            if (_idleDelta > 4)
            {
                _idleDelta = 0;
                _agent.RandomMove();
            }
            if(_translateDelta > 1)
            {
                _translateDelta = 0;
                if(_context.CanReact(_agent.Team)) _agent.TranslateState(StateType.CaReact);
                else
                {
                    var aim = _context.GetNealyCounter(_agent.transform, _agent.Team);
                    if (aim != null && (aim.position - _agent.transform.position).sqrMagnitude < 36)
                    {
                        _agent.TranslateState(StateType.CaReact);
                    }
                }
            }
        }
    }
}
