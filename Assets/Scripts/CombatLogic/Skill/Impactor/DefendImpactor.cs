using Assets.Scripts.CombatLogic.Characters;
using Assets.Scripts.Entities;
using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.Skill.Impactor
{
    internal class DefendImpactor : IImpactor
    {
        float defendFactor = 0.5f;
        float duration = 1;
        public void Impact(Transform aim)
        {
            aim.GetComponent<OperatorController>().Model.DefendFactor += defendFactor;
            DOVirtual.DelayedCall(duration, () =>
            {
                aim.GetComponent<OperatorController>().Model.DefendFactor -= defendFactor;
            });
        }

        public void Init(SkillImpactor impactor, Transform castor)
        {
            
        }
    }
}
