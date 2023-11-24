using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.ComputerControllers.Fighter.States
{
    internal class ReturnState : IFighterState
    {
        FighterController _controller;
        public ReturnState(FighterController controller)
        {
            _controller = controller;
        }
        public void OnEnter()
        {
        }

        public void OnExit()
        {
        }

        public void OnUpdate()
        {
            _controller.SetDest(_controller.CvBase.position);
            var distance = _controller.CvBase.position - _controller.transform.position;
            distance.y = 0;
            if (distance.magnitude < 2)
            {
                // 模拟回复子弹
                _controller.TranslateState(IFighterState.StateType.Idle);
            }

        }
    }
}
