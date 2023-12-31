﻿using UnityEngine;

namespace Assets.Scripts.CombatLogic.Characters.Computer.Agent.States
{
    public class CaAttack : IAgentState
    {
        private AgentController _agent;
        public CaAttack(AgentController agent)
        {
            _agent = agent;
        }
        float diff_factor;
        public void OnEnter()
        {
            _agent.StopMoving();
            diff_factor = 2f;
        }

        public void OnExit()
        {
            // do nothing
        }

        public void OnUpdate()
        {
            if (_agent.TrySeeAim(_agent.aimTran))
            {
                _agent.Aim(true, _agent.aimTran.position);
                _agent.Shoot(new Vector3(_agent.aimTran.position.x, 0.8f, _agent.aimTran.position.z), diff_factor);
            }
            else
            {
                _agent.Aim(false, Vector3.zero);
                _agent.TranslateState(StateType.CaReact);
            }
            if (diff_factor > 0)
            {
                diff_factor -= diff_factor * Time.deltaTime * 0.5f; // 2秒后矫正？
            }
            if (diff_factor < 0)
            {
                diff_factor = 0;
            }
        }
    }
}
