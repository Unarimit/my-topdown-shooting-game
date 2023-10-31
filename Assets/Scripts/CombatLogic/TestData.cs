using Assets.Scripts.CombatLogic.CombatEntities;
using Assets.Scripts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CombatLogic
{
    public static class TestData
    {
        public static void AddTestData(Dictionary<Transform, CombatOperator> Operators, List<Transform> PlayerTeamTrans, List<Transform> EnemyTeamTrans)
        {
            var op1 = new Operator { HP = 10, RecoverHP = 2 };
            var op2 = new Operator { HP = 5, RecoverHP = 2 };
            foreach (var x in PlayerTeamTrans)
            {
                Operators.Add(x, new CombatOperator(op1, 0));
            }
            foreach (var x in EnemyTeamTrans)
            {
                Operators.Add(x, new CombatOperator(op2, 1));
            }

            Operators[PlayerTeamTrans[0]].CombatSkillList.Add(new CombatCombatSkill(SkillManager.Instance.skillConfig.CombatSkills[0]));
            Operators[PlayerTeamTrans[0]].CombatSkillList.Add(new CombatCombatSkill(SkillManager.Instance.skillConfig.CombatSkills[1]));

            StorageManager.Instance.InitSet(EnemyTeamTrans.Count);
        }
    }
}
