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
        Transform _castor;
        int dmg;
        public void Init(SkillImpactor impactor, Transform castor)
        {
            _castor = castor;
            dmg = int.Parse(impactor.Data);
        }

        public void Impact(Transform aim)
        {
            CombatContextManager.Instance.DellDamage(_castor, aim, dmg);
        }

    }
}
