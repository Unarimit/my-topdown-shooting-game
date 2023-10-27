using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Entities
{
    public enum SkillEffectType
    {
        /// <summary>
        /// 使用粒子系统
        /// </summary>
        PaticalPrefab,
        /// <summary>
        /// 扔出去（抛物线，受重力影响）
        /// </summary>
        Throw,
        /// <summary>
        /// 射出去（直线，不受重力影响）
        /// </summary>
        Shoot
    }
}
