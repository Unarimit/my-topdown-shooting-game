using Assets.Scripts.CombatLogic.ContextExtends;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.Characters.Computer.Agent.States
{
    internal class CvIdle : IAgentState
    {
        private AgentController _agent;
        public CvIdle(AgentController agent)
        {
            _agent = agent;
        }
        public void OnEnter()
        {
            _idleDelta = 0;
            _agent.RandomMove();
        }

        public void OnExit()
        {
            // do nothing
        }

        private float _idleDelta = 0;
        public void OnUpdate()
        {
            
            _idleDelta += Time.deltaTime;
            if (_idleDelta > 3)
            {
                _agent.TranslateState(StateType.CvFollow);
                _idleDelta = 0;
            }
        }
    }
}
