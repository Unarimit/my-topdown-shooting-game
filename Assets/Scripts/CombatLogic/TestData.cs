using Assets.Scripts.CombatLogic.CombatEntities;
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
            foreach (var x in PlayerTeamTrans)
            {
                Operators.Add(x, new CombatOperator { HP = 10, Team = 0 });
                Operators[x].CurrentHP = Operators[x].HP;
            }
            foreach (var x in EnemyTeamTrans)
            {
                Operators.Add(x, new CombatOperator { HP = 5, Team = 1 });
                Operators[x].CurrentHP = Operators[x].HP;
            }

            Operators[PlayerTeamTrans[0]].CombatSkillList.Add(new CombatCombatSkill(SkillManager.Instance.skillConfig.CombatSkills[0]));
            Operators[PlayerTeamTrans[0]].CombatSkillList.Add(new CombatCombatSkill(SkillManager.Instance.skillConfig.CombatSkills[1]));
        }
    }
}
