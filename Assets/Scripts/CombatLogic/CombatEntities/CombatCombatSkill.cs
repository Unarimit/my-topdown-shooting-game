using Assets.Scripts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CombatLogic
{
    /// <summary>
    /// 战斗中的战斗技能
    /// </summary>
    public class CombatCombatSkill
    {
        /// <summary>
        /// 技能信息
        /// </summary>
        public CombatSkill SkillInfo { get; set; }

        public Transform Caster { get; set; }
        /// <summary>
        /// 冷却结束的准确时间
        /// </summary>
        public float CoolDownEndTime { get; set; } = -1;
        public CombatCombatSkill(CombatSkill skill)
        {
            SkillInfo = skill;
        }
        

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
