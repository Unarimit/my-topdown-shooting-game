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
        public Operator BaseInfo { get; }
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
        /// 最后一次进入交战状态的时间
        /// </summary>
        public float LastInCombatTime { get; private set; }

        //public Transform Transform { get; set; }  //暂时没用，以后有用

        public List<CombatCombatSkill> CombatSkillList { get; set; } = new List<CombatCombatSkill>();

        public CombatOperator(Operator op, int team)
        {
            BaseInfo = op;
            CurrentHP = op.HP;
            MaxHP = op.HP;
            Team = team;
        }

        public void TakeDamage(int dmg)
        {
            LastInCombatTime = Time.time;
            CurrentHP -= dmg;
        }
        public bool TryRecover()
        {
            if (Time.time - LastInCombatTime > 5) CurrentHP = Math.Min(BaseInfo.RecoverHP + CurrentHP, MaxHP);
            return Time.time - LastInCombatTime > 5;
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
