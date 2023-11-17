using Assets.Scripts.CombatLogic.CombatEntities;
using Assets.Scripts.CombatLogic.EnviormentLogic;
using Assets.Scripts.Common;
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
        public static void AddTestData(Dictionary<Transform, CombatOperator> Operators, List<Transform> PlayerTeamTrans)
        {
            var op1 = new Operator { HP = 10, RecoverHP = 2 };
            var op2 = new Operator { HP = 5, RecoverHP = 2 };
            foreach (var x in PlayerTeamTrans)
            {
                Operators.Add(x, new CombatOperator(op1, 0, CombatContextManager.Instance.Enviorment));
            }
            

            Operators[PlayerTeamTrans[0]].CombatSkillList.Add(new CombatCombatSkill(SkillManager.Instance.skillConfig.CombatSkills[0]));
            Operators[PlayerTeamTrans[0]].CombatSkillList.Add(new CombatCombatSkill(SkillManager.Instance.skillConfig.CombatSkills[1]));
            
            CombatContextManager.Instance.GenerateAgent(
                op1,
                new Vector3(1, 0, 0), new Vector3(),
                0, CombatContextManager.Instance.Enviorment);

            CombatContextManager.Instance.GenerateAgent(
                op1,
                new Vector3(2, 0, 0), new Vector3(),
                0, CombatContextManager.Instance.Enviorment);

        }

        public static List<Operator> GetTeamUnit()
        {
            var op1 = new Operator { HP = 10, RecoverHP = 2, ModelResourceUrl = "Hoshino" };
            return new List<Operator>{ op1, op1, op1, op1 };
        }

        public static int[][] GetTerrains()
        {
            return MapGenerator.RandomMap();
        }
    }
}
