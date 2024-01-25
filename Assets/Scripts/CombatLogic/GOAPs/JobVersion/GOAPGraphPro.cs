using System.Collections.Generic;
using Unity.Collections;

namespace Assets.Scripts.CombatLogic.GOAPs.JobVersion
{
    internal struct GOAPGraphPro
    {
        public NativeArray<GOAPAction> Actions;
        public List<GOAPAction> DoPlan(uint initState) {
            return GOAPGraphHelper.DoPlanForActions(initState, Actions);
        }
    }
}
