using Assets.Scripts.CombatLogic.Skill.Impactor;
using Assets.Scripts.CombatLogic.Skill.Releaser;
using System.Collections.Generic;

namespace Assets.Scripts.CombatLogic.Skill.Selector
{
    internal class SelfSelector : ISelector
    {
        public void Init(List<IImpactor> impectors, BaseReleaser releaser)
        {
            
            foreach(var x in impectors)
            {
                x.Impact(releaser.Caster);
            }
        }
    }
}
