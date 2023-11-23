using Assets.Scripts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.CombatEntities
{
    public class CombatOperator
    {
        public Operator OpInfo { get; }

        /// <summary>
        /// 出生点
        /// </summary>
        public Transform SpawnBase { get; }
        /// <summary>
        /// 目前HP
        /// </summary>
        public int CurrentHP { get; private set; }
        /// <summary>
        /// 战斗中最大HP
        /// </summary>
        public int MaxHP { get; private set; }
        public float Speed { get; set; }
        public int Team { get; private set; }

        /// <summary>
        /// 复活剩余时间
        /// </summary>
        public float CurrentReviveTime { get; private set; }

        public bool IsDead => CurrentReviveTime != 0;

        /// <summary>
        /// 最后一次进入交战状态的时间
        /// </summary>
        public float LastInCombatTime { get; private set; }

        //public Transform Transform { get; set; }  //暂时没用，以后有用

        /// <summary>
        /// 战斗技能
        /// </summary>
        public List<CombatCombatSkill> CombatSkillList { get; set; } = new List<CombatCombatSkill>();

        /// <summary>
        /// 武器技能
        /// </summary>
        public CombatCombatSkill WeaponSkill { get; private set; }

        public CombatOperator(Operator op, int team, Transform spawnBase)
        {
            OpInfo = op;
            MaxHP = op.McBody.HP;
            CurrentHP = MaxHP;
            Team = team;
            SpawnBase = spawnBase;
            WeaponSkill = new CombatCombatSkill(SkillManager.Instance.skillConfig.CombatSkills[op.WeaponSkillId]);
            CombatSkillList.Add(new CombatCombatSkill(SkillManager.Instance.skillConfig.CombatSkills[op.MainSkillId]));
        }

        public void TakeDamage(int dmg)
        {
            LastInCombatTime = Time.time;
            CurrentHP -= dmg;
        }
        public bool TryRecover()
        {
            if (IsDead) return false;
            if (Time.time - LastInCombatTime > 5) CurrentHP = Math.Min(OpInfo.RecoverHP + CurrentHP, MaxHP);
            return Time.time - LastInCombatTime > 5;
        }

        /// <summary>
        /// 刷新复活时间，如果返回true表示复活时间正好清零
        /// </summary>
        /// <returns></returns>
        public bool TryRevive()
        {
            if (!IsDead) return false; // 没死不能复活
            if (SpawnBase == null) return false; // 家没了不能复活
            CurrentReviveTime -= Time.deltaTime;
            if (CurrentReviveTime < 0)
            {
                CurrentReviveTime = 0;
                return true;
            }
            else
            {
                return false;
            }
        }
        public void DoDied()
        {
            CurrentReviveTime = OpInfo.ReviveTime;
        }

        public void Respawn()
        {
            CurrentHP = MaxHP;
        }

        /// <summary>
        /// 发生主动战斗行为
        /// </summary>
        public void ActAttack()
        {
            LastInCombatTime = Time.time;
        }
    }
}
