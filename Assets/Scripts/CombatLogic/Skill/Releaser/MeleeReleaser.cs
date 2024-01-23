using Assets.Scripts.CombatLogic.Skill.Impactor;
using Assets.Scripts.CombatLogic.Skill.Selector;
using Assets.Scripts.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.Skill.Releaser
{
    internal class MeleeReleaser : BaseReleaser
    {
        private CombatSkill _skill;
        private Transform _caster;
        private Vector3 _aim;
        public override void Release(Transform caster, CombatSkill skill, Vector3 aim)
        {
            _skill = skill;
            // 配置selector
            var selector = createSelector(skill.SkillSelector.SelectorName);

            // 配置impector
            var impactors = new List<IImpactor>();
            foreach (var im in skill.SkillImpectors)
            {
                impactors.Add(createImpactor(im.ImpectorName));
                impactors[^1].Init(im, caster);
            }


            selector.Init(impactors, caster, skill, aim);


            // 延迟销毁go和触发连锁技能
            StartCoroutine(DelayDestroySelf()); // 可能因为角色死亡而异常
        }

        

        IEnumerator DelayDestroySelf()
        {
            yield return new WaitForSeconds(_skill.Duration);
            if (_skill.IsHaveNextSkill) InvokeTriggerChainSkillEvent(_caster, SkillManager.Instance.skillConfig.CombatSkills[_skill.NextSkillId], _aim, transform.position, transform.eulerAngles);
            Destroy(this); // 销毁component而不是go
            yield break;
        }
    }
}
