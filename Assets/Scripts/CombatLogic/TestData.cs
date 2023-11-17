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
        public static List<Operator> GetTeamUnit()
        {
            return TestDB.GetRandomOperator(5);
        }

        public static int[][] GetTerrains()
        {
            return MapGenerator.RandomMap();
        }
    }
}
