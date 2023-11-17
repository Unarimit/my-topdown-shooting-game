using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    public class LevelInfo
    {
        public string LevelName;
        public string WinDesc;
        public string LossDesc;
        public int[][] Map;
        public List<Operator> EnemyOperators;
        public RectInt EnemySpawn;

        public List<Operator> TeamOperators;
        public RectInt TeamSpawn;
    }
}
