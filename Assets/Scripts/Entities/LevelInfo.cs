using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    public class LevelInfo
    {
        public LevelRule LevelRule;
        public int[][] Map;
        public List<Operator> EnemyOperators;
        public RectInt EnemySpawn;

        public List<Operator> TeamOperators;
        public RectInt TeamSpawn;

        public string GetWinDesc()
        {
            var sb = new StringBuilder();
            foreach(var con in LevelRule.WinCondition)
            {
                sb.Append(string.Format(con.Description,con.Amount.ToString()));
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
