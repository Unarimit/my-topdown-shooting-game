using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    /// <summary>
    /// 战斗技能
    /// </summary>
    [Serializable]
    public class CombatSkill
    {
        /// <summary>
        /// 技能类型
        /// </summary>
        public SkillType Type;
        /// <summary>
        /// 技能名称
        /// </summary>
        public string Name;

        /// <summary>
        /// 技能Id
        /// </summary>
        public int Id;

        /// <summary>
        /// 技能描述
        /// </summary>
        public string Description;

        /// <summary>
        /// 技能冷却
        /// </summary>
        public float CoolDown;

        /// <summary>
        /// 技能持续时间
        /// </summary>
        public float Duration;

        /// <summary>
        /// 伤害类型
        /// </summary>
        public SkillDamageType DamageType;

        /// <summary>
        /// 技能造成伤害
        /// </summary>
        public int Damage;

        /// <summary>
        /// 链式技能触发，在持续时间结束后触发。如果没有下一个，设为-1
        /// </summary>
        public int NextSkillId = -1;

        /// <summary>
        /// 最大使用次数，-1表示无限
        /// </summary>
        public int MaxUse = -1;

        /// <summary>
        /// 技能效果类型
        /// </summary>
        public SkillEffectType EffectType;

        /// <summary>
        /// 技能Prefab路径
        /// </summary>
        public string PrefabResourceUrl;

        /// <summary>
        /// 技能Icon路径
        /// </summary>
        public string IconUrl;

        /// <summary>
        /// 技能影响组
        /// </summary>
        public SkillAffectGroup AffectGroup;

        public bool IsHaveNextSkill  => NextSkillId != -1;
    }
}
