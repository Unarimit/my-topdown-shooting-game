using Assets.Scripts.CombatLogic.Skill.Releaser;
using Assets.Scripts.Entities;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.Skill.Impactor
{
    internal interface IImpactor
    {
        public void Init(SkillImpactor impactor, BaseReleaser releaser);
        public void Impact(Transform aim);
    }
}
