using Assets.Scripts.CombatLogic.Skill.Impactor;
using Assets.Scripts.CombatLogic.Skill.Selector;
using Assets.Scripts.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.Skill.Releaser
{
    /// <summary>
    /// 贴在角色go上的释放器，没有（衍生）投掷物
    /// </summary>
    internal class MeleeReleaser : BaseReleaser
    {
        public override void Release(Transform caster, CombatSkill skill, Vector3 aim)
        {
            Skill = skill;
            Caster = caster;
            Aim = aim;
            // 配置selector
            var selector = createSelector(skill.SkillSelector.SelectorName);

            // 配置impector
            var impactors = new List<IImpactor>();
            foreach (var im in skill.SkillImpectors)
            {
                impactors.Add(createImpactor(im.ImpectorName));
                impactors[^1].Init(im, this);
            }


            selector.Init(impactors, this);


            // 延迟销毁go和触发连锁技能
            if(gameObject.activeSelf is true)
                StartCoroutine(DelayDestroySelf()); 
            else
                Destroy(this); // 可因角色死亡提前销毁
        }

        IEnumerator DelayDestroySelf()
        {
            yield return new WaitForSeconds(Skill.Duration);
            if (Skill.IsHaveNextSkill) InvokeTriggerChainSkillEvent(Caster, SkillManager.Instance.skillConfig.CombatSkills[Skill.NextSkillId], Aim, transform.position, transform.eulerAngles);
            Destroy(this); // 销毁component而不是go
            yield break;
        }
    }
}
