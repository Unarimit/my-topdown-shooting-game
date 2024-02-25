using Assets.Scripts.CombatLogic.Skill.Impactor;
using Assets.Scripts.CombatLogic.Skill.Releaser;
using System.Collections.Generic;

namespace Assets.Scripts.CombatLogic.Skill.Selector
{
    internal interface ISelector
    {
        public void Init(List<IImpactor> impectors, BaseReleaser releaser);
    }
}
