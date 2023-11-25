using Assets.Scripts.CombatLogic.Skill.Impactor;
using Assets.Scripts.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.Skill.Selector
{
    internal interface ISelector
    {
        public void Init(List<IImpactor> impectors, Transform caster, CombatSkill skill, Vector3 aim);
    }
}
