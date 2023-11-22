using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.ContextExtends
{
    static internal class AiInformationHelper
    {
        /// <summary>
        /// 尝试发现敌人
        /// </summary>
        /// <returns></returns>
        public static Transform GetACounter(this CombatContextManager context, int belongTeam)
        {
            List<Transform> CounterGroup;
            if (belongTeam == 0) CounterGroup = context.EnemyTeamTrans;
            else CounterGroup = context.PlayerTeamTrans;

            if (CounterGroup == null || CounterGroup.Count == 0) return null;

            return CounterGroup[Random.Range(0, CounterGroup.Count)];
        }
    }
}
