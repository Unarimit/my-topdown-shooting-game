using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CombatLogic.GOAPs
{
    internal struct GOAPAction
    {
        public uint Conditions;
        public uint Effects;
        public (GOAPStatus, float)[] Factors;
        public float Cost;
        public GOAPPlan GOAPPlan;

        // DEBUG
        public string ActionName;

        public bool IsMatchCondtions(uint state)
        {
            return Conditions == 0 ? true : (state & Conditions) != 0;
        }

        public uint Transfer(uint state)
        {
            return state | Effects;
        }

        public float GetCost(uint state)
        {
            float res = Cost;
            foreach(var fac in Factors)
            {
                if ((state & ((uint)1 << (int)fac.Item1)) != 0) res += fac.Item2;
            }
            return res;
        }
    }
}
