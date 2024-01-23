using Assets.Scripts.Entities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.CombatEntities
{
    public class CombatOperator
    {
        public Operator OpInfo { get; }

        //public Transform Transform { get; set; }  //暂时没用

        /// <summary>
        /// 标识id，随构造函数自增
        /// </summary>
        public int Id { get; }

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
        /// <summary>
        /// 战斗中最大速度
        /// </summary>
        public float Speed { get; private set; }

        /// <summary>
        /// 所属队伍 目前：0：玩家； 1：敌人
        /// </summary>
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

        public bool IsPlayer { get; set; }
        /// <summary>
        /// 战斗技能
        /// </summary>
        public List<CombatCombatSkill> CombatSkillList { get; set; } = new List<CombatCombatSkill>();

        /// <summary>
        /// 武器技能
        /// </summary>
        public CombatCombatSkill WeaponSkill { get; private set; }
        /// <summary>
        /// 按c键时的技能
        /// </summary>
        public CombatCombatSkill SlideSkill { get; private set; }

        #region AI信息
        /// <summary> 攻击范围(根据武器技能判断) </summary>
        public float AttackRange { get; }

        /// <summary> 视野范围(根据模板判断) </summary>
        public float SeeRange { get; }
        #endregion


        #region 战斗Buffs
        public float DefendFactor { get; set; } = 0;
        #endregion

        #region 战斗统计信息
        public int StatCauseDamage { get; private set; } = 0;
        public int StatReceiveDamage { get; private set; } = 0;

        #endregion

        private static int _gId = 0; 
        public CombatOperator(Operator op, int team, Transform spawnBase, bool isPlayer)
        {
            Id = ++_gId;
            if (Id == int.MaxValue) throw new ArgumentOutOfRangeException("auto incrasement id overflow");
            OpInfo = op;
            MaxHP = op.McBody.HP;
            CurrentHP = MaxHP;
            Team = team;
            SpawnBase = spawnBase;
            Speed = op.McLeg.Speed;
            WeaponSkill = new CombatCombatSkill(SkillManager.Instance.skillConfig.CombatSkills[op.WeaponSkillId]);
            SlideSkill = new CombatCombatSkill(SkillManager.Instance.skillConfig.CombatSkills[op.SlideSkillId]);
            CombatSkillList.Add(new CombatCombatSkill(SkillManager.Instance.skillConfig.CombatSkills[op.MainSkillId]));
            IsPlayer = isPlayer;
            AttackRange = WeaponSkill.SkillInfo.RangeTip;
            SeeRange = 15f;
        }

        public int TakeDamage(int dmg)
        {
            LastInCombatTime = Time.time;
            int realDmg = (int)Mathf.Ceil(dmg * (1 - DefendFactor));
            if (CurrentHP - realDmg > MaxHP)
            {
                CurrentHP = MaxHP;
                realDmg = CurrentHP - MaxHP;
            }
            CurrentHP -= realDmg;
            StatReceiveDamage += Math.Abs(realDmg);
            return realDmg;
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
        public void ActAttack(int val)
        {
            LastInCombatTime = Time.time;
            StatCauseDamage += Math.Abs(val);
        }
    }
}
