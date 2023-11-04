using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CombatLogic.ComputerControllers.States
{
    public enum StateType
    {
        Idle, React, Attack
    }

    public interface IAgentState
    {
        void OnEnter();

        void OnUpdate();

        void OnExit();
    }
}
