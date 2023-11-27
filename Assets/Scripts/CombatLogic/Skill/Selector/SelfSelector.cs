using Assets.Scripts.CombatLogic.Skill.Impactor;
using Assets.Scripts.Entities;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.Skill.Selector
{
    internal class SelfSelector : ISelector
    {
        public void Init(List<IImpactor> impectors, Transform caster, CombatSkill skill, Vector3 aim)
        {
            
            foreach(var x in impectors)
            {
                x.Impact(caster);
            }
        }
    }
}
