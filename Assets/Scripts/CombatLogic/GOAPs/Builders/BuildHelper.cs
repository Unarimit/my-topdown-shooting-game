using Assets.Scripts.CombatLogic.CombatEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CombatLogic.GOAPs.Builders
{
    internal static class BuildHelper
    {
        public static GOAPGraph BuildActionGraphForAgent(CombatOperator cop)
        {
            var builder = new GOAPGraphBuilder($"{cop.OpInfo.Name}:{cop.Id} GOAPGraph");
            addCommonsenceAction(builder);

            builder.AddAction("patrol")
                .SetCost(20)
                .SetPlan(GOAPPlan.Patrol)
                .AddEffect(GOAPStatus.SawEnemy)
                .BuildAction();

            builder.AddAction("reload")
                .SetCost(30)
                .AddCondition(GOAPStatus.UseGun)
                .AddFactor(GOAPStatus.LowAmmo, -10)
                .AddFactor(GOAPStatus.NoAmmo, -15)
                .SetPlan(GOAPPlan.RetreatAndReload)
                .AddEffect(GOAPStatus.Escape)
                .BuildAction();

            if (cop.WeaponSkill.SkillInfo.TargetTip == Entities.SkillTargetTip.EnemySingle)
            {
                builder.AddAction("go and attack")
                    .SetCost(5)
                    .AddCondition(GOAPStatus.SawEnemy)
                    .AddEffect(GOAPStatus.Kill)
                    .AddFactor(GOAPStatus.LowAmmo, 5)
                    .AddFactor(GOAPStatus.NoAmmo, 20)
                    .SetPlan(GOAPPlan.GoAndAttack)
                    .BuildAction();
                builder.AddAction("surrond and attack")
                    .SetCost(8)
                    .AddCondition(GOAPStatus.SawEnemy)
                    .AddEffect(GOAPStatus.Kill)
                    .AddFactor(GOAPStatus.Tactical, -5)
                    .AddFactor(GOAPStatus.LowAmmo, 5)
                    .AddFactor(GOAPStatus.NoAmmo, 20)
                    .SetPlan(GOAPPlan.SurrondAndAttack)
                    .BuildAction();
            }
            else if(cop.WeaponSkill.SkillInfo.TargetTip == Entities.SkillTargetTip.TeammateSingle)
            {
                builder.AddAction("follow and heal")
                    .SetCost(5)
                    .AddEffect(GOAPStatus.Support)
                    .SetPlan(GOAPPlan.FollowAndHeal)
                    .BuildAction();
            }
            else
            {
                throw new ArgumentException($"not setting adjust action for TargetTip:{cop.WeaponSkill.SkillInfo.TargetTip}");
            }

            return builder.BuildGraph();
        }
        private static void addCommonsenceAction(GOAPGraphBuilder builder)
        {
            builder.AddAction("final-win")
                .AddCondition(GOAPStatus.Win)
                .SetGoal()
                .SetCost(0)
                .BuildAction();

            builder.AddAction("abstart-kill")
                .AddCondition(GOAPStatus.Kill)
                .AddEffect(GOAPStatus.Win)
                .SetCost(2)
                .BuildAction();

            builder.AddAction("abstart-support")
                .AddCondition(GOAPStatus.Support)
                .AddEffect(GOAPStatus.Win)
                .SetCost(5)
                .BuildAction();

            builder.AddAction("abstart-escape")
                .AddCondition(GOAPStatus.Escape)
                .AddEffect(GOAPStatus.Win)
                .AddFactor(GOAPStatus.LowHP, -10)
                .AddFactor(GOAPStatus.LowAmmo, -10)
                .SetCost(20)
                .BuildAction();
        }
    }
}
