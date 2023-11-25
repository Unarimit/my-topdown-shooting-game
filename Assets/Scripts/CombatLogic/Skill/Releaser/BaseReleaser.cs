using Assets.Scripts.CombatLogic.Skill.Impactor;
using Assets.Scripts.CombatLogic.Skill.Selector;
using Assets.Scripts.Entities;
using System;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.Skill.Releaser
{
    /// <summary>
    /// 技能释放器，创建特效，根据技能描述选择器和效果器
    /// </summary>
    internal class BaseReleaser : MonoBehaviour
    {
        public virtual void Release(Transform Caster, CombatSkill skill, Vector3 aim)
        {

        }

        protected void InvokeTriggerChainSkillEvent(Transform Caster, CombatSkill skill, Vector3 aim, Vector3 startPos, Vector3 startAngle)
        {
            TriggerChainSkillEventHandler.Invoke(Caster, skill, aim, startPos, startAngle);
        }

        protected static ISelector createSelector(string name)
        {
            string realName = $"Assets.Scripts.CombatLogic.Skill.Selector.{name}Selector";
            var selector = Activator.CreateInstance(Type.GetType(realName));
            if (selector == null || selector is not ISelector)
            {
                Debug.LogError($"Releaser can not load selector:{realName}");
                return default(ISelector);
            }
            else
            {
                return (ISelector)selector;
            }
        }
        protected static IImpactor createImpactor(string name)
        {
            string realName = $"Assets.Scripts.CombatLogic.Skill.Impactor.{name}Impactor";
            var impactor = Activator.CreateInstance(Type.GetType(realName));
            if (impactor == null || impactor is not IImpactor)
            {
                Debug.LogError($"Releaser can not load impactor:{realName}");
                return default(IImpactor);
            }
            else
            {
                return (IImpactor)impactor;
            }
        }

        public delegate void TriggerChainSkillEvent(Transform Caster, CombatSkill skill, Vector3 aim, Vector3 startPos, Vector3 startAngle);
        public event TriggerChainSkillEvent TriggerChainSkillEventHandler;
    }
}
