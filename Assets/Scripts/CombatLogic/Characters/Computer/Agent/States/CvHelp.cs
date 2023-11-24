using Assets.Scripts.CombatLogic.ContextExtends;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.Characters.Computer.Agent.States
{
    internal class CvHelp : IAgentState
    { 

        AgentController _agent;
        CombatContextManager _context => CombatContextManager.Instance;
        Transform aim;
        public CvHelp(AgentController agent)
        {
            _agent = agent;
        }
        public void OnEnter()
        {
            aim = _context.GetNealyFriend(_agent.transform, _agent.Team);
            if (aim == null) _agent.TranslateState(StateType.CvFollow);
        }

        public void OnExit()
        {
            // do nothing
        }

        public void OnUpdate()
        {

            if (_context.Operators[aim].CurrentHP != _context.Operators[aim].MaxHP)
            {
                _agent.Aim(aim.position);
                _agent.Shoot(new Vector3(aim.position.x, 0.8f, aim.position.z));
            }
            else
            {
                _agent.TranslateState(StateType.CvIdle);
            }
        }
    }
}
