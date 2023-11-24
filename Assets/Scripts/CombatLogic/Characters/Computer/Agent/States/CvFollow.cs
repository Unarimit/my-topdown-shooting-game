using Assets.Scripts.CombatLogic.ContextExtends;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.Characters.Computer.Agent.States
{
    internal class CvFollow : IAgentState
    {
        private AgentController _agent;
        private Transform follow;
        CombatContextManager _context => CombatContextManager.Instance;
        public CvFollow(AgentController agent)
        {
            _agent = agent;
        }

        public void OnEnter()
        {
            follow = _context.GetAFriend(_agent.transform, _agent.Team);
            if(follow == null)
            {
                _agent.TranslateState(StateType.CvIdle);
                return;
            }
            _agent.NavMeshAgent.stoppingDistance = 3;
            _agent.MoveTo(follow.position, 1);
        }

        public void OnExit()
        {

        }

        private float _idleDelta = 0;
        public void OnUpdate()
        {
            if (_context.Operators[follow].MaxHP != _context.Operators[follow].CurrentHP)
            {
                _agent.TranslateState(StateType.CvHelp);
                return;
            }

            _idleDelta += Time.deltaTime;
            if (_idleDelta > 0.2)
            {
                _agent.MoveTo(follow.position, 1);
                _idleDelta = 0;
            }
        }
    }
}
