using Assets.Scripts.CombatLogic.ContextExtends;
using Assets.Scripts.CombatLogic.Skill.Impactor;
using Assets.Scripts.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.Skill.Selector
{
    internal class SectorSelector : ISelector
    {
        float distance = 2;
        float angle = 30;
        public void Init(List<IImpactor> impectors, Transform caster, CombatSkill skill, Vector3 aim)
        {
            var res = CombatContextManager.Instance.FindOpTransform(x =>
                Vector3.Distance(caster.position, x.position) < distance &&
                Vector3.Angle(caster.position, x.position) < angle &&
                CombatContextManager.Instance.Operators[x].IsDead is false
            );

            foreach(var x in res)
            {
                foreach(var im in impectors)
                {
                    im.Impact(x);
                }
            }
        }
    }
}
