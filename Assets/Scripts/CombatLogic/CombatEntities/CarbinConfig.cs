using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.CombatEntities
{
    /// <summary>
    /// agent的出生地
    /// </summary>
    [Serializable]
    public class CarbinConfig
    {
        /// <summary>
        /// 所属队伍
        /// </summary>
        public int Team;

        /// <summary>
        /// 最大数量
        /// </summary>
        public int MaxCapacity;

        /// <summary>
        /// 出生间隔
        /// </summary>
        public float SpawnInterval;

        /// <summary>
        /// agent的prefab位置
        /// </summary>
        public string AgentPrefabURL;
    }
}
