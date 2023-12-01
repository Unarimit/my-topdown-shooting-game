using Assets.Scripts.CombatLogic.Skill.Impactor;
using Assets.Scripts.CombatLogic.Skill.Selector;
using Assets.Scripts.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.Skill.Releaser
{
    internal class RangeReleaser : BaseReleaser
    {
        private CombatSkill _skill;
        private Transform _caster;
        private Vector3 _aim;
        public override void Release(Transform caster, CombatSkill skill, Vector3 aim)
        {
            _caster = caster;
            _skill = skill;
            _aim = aim;
            // 配置selector
            ISelector selector = null;
            if (skill.SkillSelector.SelectorName == TestDB.SkillSelectorStr.Trigger.ToString())
            {
                selector = gameObject.AddComponent<TriggerSelector>();
            }
            else
            {
                selector = createSelector(skill.SkillSelector.SelectorName);
            }

            // 配置impector
            var impactors = new List<IImpactor>();
            if(skill.SkillImpectors != null)
            {
                foreach (var im in skill.SkillImpectors)
                {
                    impactors.Add(createImpactor(im.ImpectorName));
                    impactors[impactors.Count - 1].Init(im, caster);
                }
            }
            


            selector.Init(impactors, caster, skill, aim);


            // 延迟销毁go和触发连锁技能
            StartCoroutine(DelayDestroySelf());
        }

        

        IEnumerator DelayDestroySelf()
        {
            yield return new WaitForSeconds(_skill.Duration);
            if (_skill.IsHaveNextSkill) InvokeTriggerChainSkillEvent(_caster, SkillManager.Instance.skillConfig.CombatSkills[_skill.NextSkillId], _aim, transform.position, transform.eulerAngles);
            Destroy(gameObject);
            yield break;
        }

        
    }
}
