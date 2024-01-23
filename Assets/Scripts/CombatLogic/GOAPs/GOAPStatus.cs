using Assets.Scripts.CombatLogic.CombatEntities;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.GOAPs
{
    internal enum GOAPStatus
    {
        // 角色状态
        /// <summary> 血量少于1/3 </summary>
        LowHP,
        /// <summary> 使用枪械 </summary>
        UseGun,
        /// <summary> 子弹少于1/3（10发子弹以上枪械才有这个属性） </summary>
        LowAmmo,
        /// <summary> 空子弹（枪械才有这个属性） </summary>
        NoAmmo,
        /// <summary> 战斗中 </summary>
        InAttack,
        /// <summary> 拥有不在cd的攻击技能 </summary>
        HaveNoCdAttackSkill,

        // 角色观察状态
        /// <summary> 观察到敌人 </summary>
        SawEnemy,
        /// <summary> 敌人在攻击范围内 </summary>
        EnemyInRange,
        /// <summary> （附近战力）我方强于敌方 </summary>
        StrongThanEnemy, // 这依赖于这一项的Plan一定是在开局规划中无法做出的，是中途规划需要改变的

        // 驱动状态
        Kill,
        Support,
        Escape,
        /// <summary> 胜利，最终状态 </summary>
        Win,

        // op性格
        /// <summary> 激进的 </summary>
        Offensive,
        /// <summary> 战略家 </summary>
        Tactical,
        /// <summary> 胆小的 </summary>
        Timid,

    }

    internal class GOAPStatusHelper
    {
        /// <summary>
        /// 计算GOAP世界状态
        /// </summary>
        public static uint CalcState(CombatOperator cop, GunController.GunProperty gunProperty, bool inAttack, bool canSeeEnemy, bool isEnemyInRange)
        {
            uint res = 0;
            /*****************  简单状态  *****************/
            // 角色状态
            // -- HP
            if (cop.CurrentHP < cop.MaxHP / 3) res |= (uint)1 << (int)GOAPStatus.LowHP;
            // -- 武器
            if (gunProperty != null)
            {
                res |= (uint)1 << (int)GOAPStatus.UseGun;
                if (gunProperty.CurrentAmmo == 0) res |= (uint)1 << (int)GOAPStatus.NoAmmo;
                else if(gunProperty.MaxAmmo > gunProperty.CurrentAmmo * 3)
                {
                    res |= (uint)1 << (int)GOAPStatus.LowAmmo;
                }
            }
            // -- 技能
            foreach (var x in cop.CombatSkillList)
            {
                if(x.IsCoolDowning(Time.deltaTime) is false && 
                    (x.SkillInfo.TargetTip == Entities.SkillTargetTip.EnemySingle || x.SkillInfo.TargetTip == Entities.SkillTargetTip.EnemyRange))
                {
                    res |= (uint)1 << (int)GOAPStatus.HaveNoCdAttackSkill;
                }
            }

            // op性格
            if (cop.OpInfo.Trait == Entities.OperatorTrait.Offensive) res |= (uint)1 << (int)GOAPStatus.Offensive;
            else if (cop.OpInfo.Trait == Entities.OperatorTrait.Timid) res |= (uint)1 << (int)GOAPStatus.Timid;
            else if (cop.OpInfo.Trait == Entities.OperatorTrait.Tactical) res |= (uint)1 << (int)GOAPStatus.Tactical;


            // 角色观察状态比较难算
            if (inAttack) res |= (uint)1 << (int)GOAPStatus.InAttack;
            if (canSeeEnemy) res |= (uint)1 << (int)GOAPStatus.SawEnemy;
            if (isEnemyInRange) res |= (uint)1 << (int)GOAPStatus.EnemyInRange;

            return res;
        }

        
    }
}
