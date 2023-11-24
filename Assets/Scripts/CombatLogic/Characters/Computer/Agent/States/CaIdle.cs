using UnityEngine;

namespace Assets.Scripts.CombatLogic.Characters.Computer.Agent.States
{
    public class CaIdle : IAgentState
    {
        private AgentController _agent;
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

        private float _idleDelta = 0;
        public void OnUpdate()
        {
            _agent.aimPos = _agent.TryFindAim();
            if (_agent.aimPos != new Vector3())
            {
                _agent.TranslateState(StateType.CaReact);
            }
            else
            {
                _idleDelta += Time.deltaTime;
                if (_idleDelta > 3)
                {
                    _idleDelta = 0;
                    _agent.RandomMove();
                }
            }
        }
    }
}
