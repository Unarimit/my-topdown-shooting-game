using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.ComputerControllers.States
{
    public class ReactState : IAgentState
    {
        private AgentController _agent;
        public ReactState(AgentController agent)
        {
            _agent = agent;
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
            if (_agent._context.Operators[_agent.transform].Team == 1) msg = _agent.TrySeeCounters(_agent._context.PlayerTeamTrans);
            else msg = _agent.TrySeeCounters(_agent._context.EnemyTeamTrans);
            if (msg.Found == true)
            {
                _agent.transform.LookAt(msg.FoundPos);
                _agent.aimTran = msg.FoundTrans;
                _agent.TranslateState(StateType.Attack);
            }
            else if(_agent.isStopped)
            {
                _agent.TranslateState(StateType.Idle);
            }
        }
    }
}
