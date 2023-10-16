using Assets.Scripts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CombatLogic.CombatEntities
{
    /// <summary>
    /// 战斗中的战斗技能
    /// </summary>
    public class CombatCombatSkill : CombatSkill
    {

        /// <summary>
        /// 冷却结束的准确时间
        /// </summary>
        public float CoolDownEndTime { get; set; } = -1;

        /// <summary>
        /// 判断是否在冷却中
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool IsCoolDowning(float time)
        {
            if (CoolDownEndTime == -1) return false;
            return CoolDownEndTime >= time;
        }
    }
}
