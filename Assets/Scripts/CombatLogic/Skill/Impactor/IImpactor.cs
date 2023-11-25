using Assets.Scripts.Entities;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.Skill.Impactor
{
    internal interface IImpactor
    {
        public void Init(SkillImpactor impactor, Transform castor);
        public void Impact(Transform aim);
    }
}
