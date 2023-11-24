using Assets.Scripts.CombatLogic.ContextExtends;

namespace Assets.Scripts.CombatLogic.Characters.Computer.Agent.States
{
    public class CaReact : IAgentState
    {
        private AgentController _agent;
        private CombatContextManager _context;
        public CaReact(AgentController agent, CombatContextManager context)
        {
            _agent = agent;
            _context = context;
        }
        public void OnEnter()
        {
            _agent.aimTran = _context.GetACounter(_agent.Team);
            if(_agent.aimTran == null)
            {
                _agent.TranslateState(StateType.CaIdle);
            }
            else
            {
                _agent.aimPos = _agent.aimTran.position;
                _agent.MoveTo(_agent.aimPos, 1);
            }
            
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
                _agent.TranslateState(StateType.CaAttack);
            }
            else if (_context.Operators[_agent.aimTran].IsDead)
            {
                _agent.TranslateState(StateType.CaIdle);
            }
        }
    }
}
