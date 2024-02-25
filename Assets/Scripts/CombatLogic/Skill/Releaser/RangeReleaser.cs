using Assets.Scripts.CombatLogic.Skill.Impactor;
using Assets.Scripts.CombatLogic.Skill.Selector;
using Assets.Scripts.Entities;
using Assets.Scripts.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.Skill.Releaser
{
    /// <summary>
    /// 远程释放器，有（衍生）投掷物
    /// </summary>
    internal class RangeReleaser : BaseReleaser
    {
        ISelector selector;
        List<IImpactor> impactors = new List<IImpactor>();
        /// <summary>
        /// 释放，考虑重用时的情况
        /// </summary>
        public override void Release(Transform caster, CombatSkill skill, Vector3 aim)
        {
            Caster = caster;
            Skill = skill;
            Aim = aim;
            // 配置selector
            if (skill.SkillSelector.SelectorName == MyConfig.SkillSelectorStr.Trigger.ToString()) // 两个有投射物（以来碰撞箱）的特殊selector
            {
                selector = gameObject.AddComponent<TriggerSelector>();
            }
            else if(skill.SkillSelector.SelectorName == MyConfig.SkillSelectorStr.LuaTrigger.ToString())
            {
                selector = gameObject.AddComponent<LuaTriggerSelector>();
            }
            else
            {
                selector = createSelector(skill.SkillSelector.SelectorName);
            }

            // 配置impector
            if(skill.SkillImpectors != null)
            {
                foreach (var im in skill.SkillImpectors)
                {
                    impactors.Add(createImpactor(im.ImpectorName));
                    impactors[impactors.Count - 1].Init(im, this);
                }
            }
            

            // 推动技能释放过程
            selector.Init(impactors, this);


            // 延迟销毁go和触发连锁技能
            StartCoroutine(DelayDestroySelf());
        }

        

        IEnumerator DelayDestroySelf()
        {
            yield return new WaitForSeconds(Skill.Duration);
            if (Skill.IsHaveNextSkill) InvokeTriggerChainSkillEvent(Caster, SkillManager.Instance.skillConfig.CombatSkills[Skill.NextSkillId], Aim, transform.position, transform.eulerAngles);
            Destroy(gameObject);
            yield break;
        }

        
    }
}
