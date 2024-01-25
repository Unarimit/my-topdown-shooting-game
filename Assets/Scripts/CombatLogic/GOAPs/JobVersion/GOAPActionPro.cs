using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Assets.Scripts.CombatLogic.GOAPs.JobVersion
{
    internal struct GOAPActionPro
    {
        public uint Conditions;
        public uint Effects;
        public UnsafeList<GOAPActionFactor> Factors;
        public float Cost;
        public GOAPPlan GOAPPlan;

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
            foreach (var fac in Factors)
            {
                if ((state & ((uint)1 << (int)fac.Status)) != 0) res += fac.Factor;
            }
            return res;
        }
    }
}
