using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Entities.Level
{
    /// <summary>
    /// 战斗关卡详情，跨3个场景
    /// </summary>
    public class CombatLevelInfo
    {
        public CombatLevelRule LevelRule;
        public int[][] Map;
        public List<Operator> EnemyOperators;
        public List<OperatorPrefab> EnemyOperatorsBy;
        public List<Operator> TeamOperators;

        public string GetWinDesc()
        {
            var sb = new StringBuilder();
            foreach (var con in LevelRule.WinCondition)
            {
                sb.Append(string.Format(con.Description, con.Amount.ToString()));
            }
            return sb.ToString();
        }

        public string GetLossDesc()
        {
            var sb = new StringBuilder();
            foreach (var con in LevelRule.LossCondition)
            {
                sb.Append(string.Format(con.Description, con.Amount.ToString()));
            }
            return sb.ToString();
        }
    }
}
