using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Assets.Scripts.CombatLogic.GOAPs.JobVersion
{
    internal struct GOAPGraphPro
    {
        public UnsafeList<GOAPActionPro> Actions;
        public List<GOAPActionPro> DoPlan(uint initState) {
            return GOAPGraphHelper.DoPlanForActions(initState, Actions);
        }
    }
}
