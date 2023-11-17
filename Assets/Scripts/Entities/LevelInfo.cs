using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    internal class LevelInfo
    {
        public int[][] Map;
        public List<Operator> EnemyOperators;
        public RectInt EnemySpawn;

        public List<Operator> TeamOperators;
        public RectInt TeamSpawn;
    }
}
