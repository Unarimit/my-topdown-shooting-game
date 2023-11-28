using Assets.Scripts.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Common
{
    internal class LevelGenerator
    {
        public static LevelInfo GeneratorLevelInfo(LevelRule rule)
        {
            var level = new LevelInfo();
            level.LevelRule = rule;
            level.EnemySpawn = new RectInt(25, 25, 5, 5); // TODO：完善出生点指示
            level.TeamSpawn = new RectInt(5, 5, 5, 5);
            level.Map = MapGenerator.RandomMap(rule.MapSize);
            level.EnemyOperators = new List<Operator>();
            foreach (var ops in rule.OperatorPrefabs)
            {
                for (int i = 0; i < Random.Range(ops.MinAmount, ops.MaxAmount + 1); i++)
                {
                    level.EnemyOperators.Add((Operator)ops.OpInfo.Clone());
                    if (ops.UseRandomCModel)
                    {
                        level.EnemyOperators[^1].ModelResourceUrl = TestDB.GetRandomModelUrl();
                    }
                }
            }
            return level;
        }
    }
}
