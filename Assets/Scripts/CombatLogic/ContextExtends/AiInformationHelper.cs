using Assets.Scripts.CombatLogic.LevelLogic;
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

            var res = CounterGroup[Random.Range(0, CounterGroup.Count)];
            if (context.Operators[res].IsDead) return CounterGroup[Random.Range(0, CounterGroup.Count)];
            return res;
        }
        public static Transform GetAFriend(this CombatContextManager context, Transform trans, int belongTeam)
        {
            List<Transform> Group;
            if (belongTeam == 1) Group = context.EnemyTeamTrans;
            else Group = context.PlayerTeamTrans;

            if (Group == null || Group.Count == 0) return null;

            foreach(var x in Group)
            {
                if (x == trans) continue;
                else if (context.Operators[x].IsDead) continue;
                else return x; // 应用某种抽样算法? 或者多次随机, 取不到就放弃
            }
            return null;
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
                if (context.Operators[x].IsDead) continue;
                var d = (x.position - trans.position).sqrMagnitude;
                if (d < distance)
                {
                    distance = d;
                    res = x;
                }
            }
            return res;
        }

        public static Transform GetNealyFriend(this CombatContextManager context, Transform trans, int belongTeam)
        {
            List<Transform> Group;
            if (belongTeam == 1) Group = context.EnemyTeamTrans;
            else Group = context.PlayerTeamTrans;

            float distance = float.MaxValue;
            Transform res = null;
            foreach (var x in Group)
            {
                if (x == trans) continue;
                if (context.Operators[x].IsDead) continue;
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

        /// <summary>
        /// 是否允许自动索敌
        /// </summary>
        public static bool CanReact(this CombatContextManager context, int belongTeam)
        {
            if(belongTeam == 1)
            {
                return GameLevelManager.Instance.IsEnemyCanReact();
            }
            else
            {
                return true;
            }
        }

        public static Transform TrySeeCounters(this CombatContextManager context, Transform trans, int belongTeam, float findAngle, float findDistance)
        {
            var counterGroup = context.GetCounterGroup(belongTeam);
            var forward = trans.forward;
            foreach (var x in counterGroup)
            {
                if (x == null || context.Operators[x].IsDead) continue;
                var vec = x.position - trans.position;
                if (Vector3.Angle(forward, vec) < findAngle && vec.magnitude < findDistance) // in my eyes
                {
                    // 确保没有遮挡
                    Ray ray = new Ray(trans.position, vec);
                    var hits = Physics.RaycastAll(ray, vec.magnitude, LayerMask.GetMask(new string[] { "Obstacle" }));
                    if (hits.Length == 0)
                    {
                        return x;
                    }
                }
            }
            return null;
        }
    }
}
