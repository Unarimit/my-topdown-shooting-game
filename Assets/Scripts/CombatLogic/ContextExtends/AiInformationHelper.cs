using System.Collections.Generic;
using System.Linq;
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
        /// <summary>
        /// 寻找最近的敌人
        /// </summary>
        /// <returns></returns>
        public static Transform GetNealyCounter(this CombatContextManager context, Transform trans, int belongTeam)
        {
            List<Transform> CounterGroup;
            if (belongTeam == 0) CounterGroup = context.EnemyTeamTrans;
            else CounterGroup = context.PlayerTeamTrans;

            float distance = float.MaxValue;
            Transform res = null;
            foreach (var x in CounterGroup)
            {
                var d = (x.position - trans.position).sqrMagnitude;
                if (d < distance)
                {
                    distance = d;
                    res = x;
                }
            }
            return res;
        }

        public static List<Transform> GetCounterGroup(this CombatContextManager context, int belongTeam)
        {
            if (belongTeam == 0) return context.EnemyTeamTrans;
            else return context.PlayerTeamTrans;
        }
    }
}
