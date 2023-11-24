namespace Assets.Scripts.CombatLogic.Characters.Computer.Agent.States
{
    public class ReactState : IAgentState
    {
        private AgentController _agent;
        private CombatContextManager _context;
        public ReactState(AgentController agent, CombatContextManager context)
        {
            _agent = agent;
            _context = context;
        }
        public void OnEnter()
        {
            _agent.MoveTo(_agent.aimPos, 3);
        }

        public void OnExit()
        {
            // do nothing
        }

        public void OnUpdate()
        {
            AgentController.SeeMsg msg;
            if (_context.Operators[_agent.transform].Team == 1) msg = _agent.TrySeeCounters(_context.PlayerTeamTrans);
            else msg = _agent.TrySeeCounters(_context.EnemyTeamTrans);
            if (msg.Found == true)
            {
                _agent.transform.LookAt(msg.FoundPos);
                _agent.aimTran = msg.FoundTrans;
                _agent.TranslateState(StateType.Attack);
            }
            else if (_agent.isStopped)
            {
                _agent.TranslateState(StateType.Idle);
            }
        }
    }
}
