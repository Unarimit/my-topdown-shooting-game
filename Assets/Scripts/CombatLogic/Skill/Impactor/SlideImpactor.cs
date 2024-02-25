using Assets.Scripts.CombatLogic.Characters;
using Assets.Scripts.CombatLogic.Skill.Releaser;
using Assets.Scripts.Entities;
using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.Skill.Impactor
{
    internal class SlideImpactor : IImpactor
    {
        float distance = 5;
        float duration = 1;
        public void Impact(Transform aim)
        {
            aim.GetComponent<OperatorController>().OutForce = aim.forward * distance / duration;
            DOVirtual.DelayedCall(duration, () =>
            {
                aim.GetComponent<OperatorController>().OutForce = Vector3.zero;
            });
        }

        public void Init(SkillImpactor impactor, BaseReleaser releaser)
        {
            
        }
    }
}
