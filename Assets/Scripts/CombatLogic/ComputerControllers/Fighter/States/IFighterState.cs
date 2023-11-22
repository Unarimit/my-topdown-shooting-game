using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CombatLogic.ComputerControllers.Fighter.States
{
    internal interface IFighterState
    {
        public enum StateType
        {
            Idle, Attack, Return
        }

        void OnEnter();

        void OnUpdate();

        void OnExit();
    }
}
