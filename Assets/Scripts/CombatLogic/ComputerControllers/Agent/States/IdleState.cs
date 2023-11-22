using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.ComputerControllers.States.Agent
{
    public class IdleState : IAgentState
    {
        private AgentController _agent;
        public IdleState(AgentController agent)
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
                _agent.TranslateState(StateType.React);
            }
            else
            {
                _idleDelta += Time.deltaTime;
                if (_idleDelta > 1)
                {
                    _idleDelta = 0;
                    _agent.RandomMove();
                }
            }
        }
    }
}
