﻿using Assets.Scripts.CombatLogic.ContextExtends;
using Assets.Scripts.CombatLogic.Skill.Impactor;
using Assets.Scripts.CombatLogic.Skill.Releaser;
using Assets.Scripts.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.Skill.Selector
{
    internal class SectorSelector : ISelector
    {
        public void Init(List<IImpactor> impectors, BaseReleaser releaser)
        {
            // DATA: distance;angle
            var data = releaser.Skill.SkillSelector.Data.Split(";");
            if (data.Length != 2) throw new System.ArgumentException($"skill data: '{releaser.Skill.SkillSelector.Data}' can not be parse to legal SectorSelector data");
            float distance = float.Parse(data[0]);
            float angle = float.Parse(data[1]);

            var res = CombatContextManager.Instance.FindOpTransform(x =>
                Vector3.Distance(releaser.Caster.position, x.position) < distance &&
                Vector3.Angle(releaser.Caster.position, x.position) < angle &&
                x != releaser.Caster &&
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
