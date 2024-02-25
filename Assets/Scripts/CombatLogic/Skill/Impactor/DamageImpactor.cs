using Assets.Scripts.CombatLogic.Skill.Releaser;
using Assets.Scripts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.Skill.Impactor
{
    internal class DamageImpactor : IImpactor
    {
        BaseReleaser _releaser;
        int dmg;
        public void Init(SkillImpactor impactor, BaseReleaser releaser)
        {
            _releaser = releaser;
            dmg = int.Parse(impactor.Data);
        }

        public void Impact(Transform aim)
        {
            CombatContextManager.Instance.DellDamage(_releaser.Caster, aim, dmg);
        }

    }
}
