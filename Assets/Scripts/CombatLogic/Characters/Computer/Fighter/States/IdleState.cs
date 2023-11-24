using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace Assets.Scripts.CombatLogic.ComputerControllers.Fighter.States
{
    internal class IdleState : IFighterState
    {
        FighterController _controller;
        public IdleState(FighterController controller)
        {
            _controller = controller;
        }
        float idleTime = 1;
        public void OnEnter()
        {
            idleTime = 1;
        }

        public void OnExit()
        {
            
        }

        public void OnUpdate()
        {
            _controller.SetDest(_controller.CvBase.position);
            idleTime -= Time.deltaTime;
            if (idleTime < 0) _controller.TranslateState(IFighterState.StateType.Attack);
        }
    }
}
