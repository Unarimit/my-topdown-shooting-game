using Assets.Scripts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CombatLogic
{
    [CreateAssetMenu(fileName = "NewSkillList", menuName = "Create SkillList")]
    public class SkillListConfig : ScriptableObject
    {
        public List<CombatSkill> CombatSkills;
    }
}
