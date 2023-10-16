using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Entities
{
    /// <summary>
    /// 战斗技能
    /// </summary>
    public class CombatSkill
    {
        /// <summary>
        /// 技能名称
        /// </summary>
        public string SkillName { get; set; }

        /// <summary>
        /// 技能描述
        /// </summary>
        public string SkillDescription { get; set; }

        /// <summary>
        /// 技能冷却
        /// </summary>
        public float CoolDown { get; set; }
    }
}
