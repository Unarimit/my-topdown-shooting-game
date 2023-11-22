using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.ComputerControllers.States.Agent
{
    public class AttackState : IAgentState
    {
        private AgentController _agent;
        public AttackState(AgentController agent)
        {
            _agent = agent;
        }
        float diff_factor;
        public void OnEnter()
        {
            _agent.StopMoving();
            diff_factor = 0.5f;
        }

        public void OnExit()
        {
            // do nothing
        }

        public void OnUpdate()
        {
            if (_agent.TrySeeAim(_agent.aimTran))
            {
                _agent.Aim(_agent.aimTran.position);
                _agent.Shoot(new Vector3(_agent.aimTran.position.x, 0.8f, _agent.aimTran.position.z), diff_factor);
            }
            else
            {
                _agent.TranslateState(StateType.React);
            }
            if(diff_factor > 0)
            {
                diff_factor -= 0.5f * Time.deltaTime;
            }
            if (diff_factor < 0)
            {
                diff_factor = 0;
            }
        }
    }
}
