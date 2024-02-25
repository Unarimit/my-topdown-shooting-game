using Assets.Scripts.CombatLogic.Skill.Impactor;
using Assets.Scripts.CombatLogic.Skill.Selector;
using Assets.Scripts.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.Skill.Releaser
{
    /// <summary>
    /// 贴在角色go上的释放器，没有（衍生）投掷物
    /// </summary>
    internal class MeleeReleaser : BaseReleaser
    {
        ISelector selector;
        List<IImpactor> impactors;
        public override void Release(SkillManager manager, Transform caster, CombatSkill skill, Vector3 aim)
        {
            if(Manager == null) // 考虑对象池重用时的情况
            {
                base.Release(manager, caster, skill, aim);
                // 配置selector
                selector = createSelector(skill.SkillSelector.SelectorName);

                // 配置impector
                impactors = new List<IImpactor>();
                foreach (var im in skill.SkillImpectors)
                {
                    impactors.Add(createImpactor(im.ImpectorName));
                    impactors[^1].Init(im, this);
                }
            }
            else
            {
                Aim = aim; // 仍要更新aim
            }

            selector.Init(impactors, this);

            // 延迟销毁go和触发连锁技能
            if(gameObject.activeSelf is true)
                StartCoroutine(DelayDestroySelf()); 
            else
                Manager.FinalizerSkill(this); // 可因角色死亡提前销毁
        }

        IEnumerator DelayDestroySelf()
        {
            yield return new WaitForSeconds(Skill.Duration);
            if (Skill.IsHaveNextSkill) InvokeTriggerChainSkillEvent(Caster, SkillManager.Instance.skillConfig.CombatSkills[Skill.NextSkillId], Aim, transform.position, transform.eulerAngles);
            Manager.FinalizerSkill(this);
            yield break;
        }
    }
}
