using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Mechas;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    internal static class TestDB
    {

        public static LevelInfo Level { get; set; }

        public static List<Operator> GetOperators()
        {
            // TODO: these just for test
            return new List<Operator>() {
                new Operator { Name = "hoshino", ModelResourceUrl = "Hoshino", McBody = MechaBody.DefaultMecha(), McHead = MechaHead.DefaultMecha(), McLeg = MechaLeg.DefaultMecha() },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko", McBody = MechaBody.DefaultMecha(), McHead = MechaHead.DefaultMecha(), McLeg = MechaLeg.DefaultMecha() },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko", McBody = MechaBody.DefaultMecha(), McHead = MechaHead.DefaultMecha(), McLeg = MechaLeg.DefaultMecha() },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko", McBody = MechaBody.DefaultMecha(), McHead = MechaHead.DefaultMecha(), McLeg = MechaLeg.DefaultMecha() },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko", McBody = MechaBody.DefaultMecha(), McHead = MechaHead.DefaultMecha(), McLeg = MechaLeg.DefaultMecha() },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko", McBody = MechaBody.DefaultMecha(), McHead = MechaHead.DefaultMecha(), McLeg = MechaLeg.DefaultMecha() },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko", McBody = MechaBody.DefaultMecha(), McHead = MechaHead.DefaultMecha(), McLeg = MechaLeg.DefaultMecha() },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko", McBody = MechaBody.DefaultMecha(), McHead = MechaHead.DefaultMecha(), McLeg = MechaLeg.DefaultMecha() },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko", McBody = MechaBody.DefaultMecha(), McHead = MechaHead.DefaultMecha(), McLeg = MechaLeg.DefaultMecha() },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko", McBody = MechaBody.DefaultMecha(), McHead = MechaHead.DefaultMecha(), McLeg = MechaLeg.DefaultMecha() },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko", McBody = MechaBody.DefaultMecha(), McHead = MechaHead.DefaultMecha(), McLeg = MechaLeg.DefaultMecha() },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko", McBody = MechaBody.DefaultMecha(), McHead = MechaHead.DefaultMecha(), McLeg = MechaLeg.DefaultMecha() },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko", McBody = MechaBody.DefaultMecha(), McHead = MechaHead.DefaultMecha(), McLeg = MechaLeg.DefaultMecha() },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko", McBody = MechaBody.DefaultMecha(), McHead = MechaHead.DefaultMecha(), McLeg = MechaLeg.DefaultMecha() },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko", McBody = MechaBody.DefaultMecha(), McHead = MechaHead.DefaultMecha(), McLeg = MechaLeg.DefaultMecha() },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko", McBody = MechaBody.DefaultMecha(), McHead = MechaHead.DefaultMecha(), McLeg = MechaLeg.DefaultMecha() },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko", McBody = MechaBody.DefaultMecha(), McHead = MechaHead.DefaultMecha(), McLeg = MechaLeg.DefaultMecha() },
                new Operator { Name = "hoshino", ModelResourceUrl = "Hoshino", McBody = MechaBody.DefaultMecha(), McHead = MechaHead.DefaultMecha(), McLeg = MechaLeg.DefaultMecha() },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko", McBody = MechaBody.DefaultMecha(), McHead = MechaHead.DefaultMecha(), McLeg = MechaLeg.DefaultMecha() },
                new Operator { Name = "hoshino", ModelResourceUrl = "Hoshino", McBody = MechaBody.DefaultMecha(), McHead = MechaHead.DefaultMecha(), McLeg = MechaLeg.DefaultMecha() },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko", McBody = MechaBody.DefaultMecha(), McHead = MechaHead.DefaultMecha(), McLeg = MechaLeg.DefaultMecha() },
                new Operator { Name = "hoshino", ModelResourceUrl = "Hoshino", McBody = MechaBody.DefaultMecha(), McHead = MechaHead.DefaultMecha(), McLeg = MechaLeg.DefaultMecha() },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko", McBody = MechaBody.DefaultMecha(), McHead = MechaHead.DefaultMecha(), McLeg = MechaLeg.DefaultMecha() },
                new Operator { Name = "hoshino", ModelResourceUrl = "Hoshino", McBody = MechaBody.DefaultMecha(), McHead = MechaHead.DefaultMecha(), McLeg = MechaLeg.DefaultMecha() },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko", McBody = MechaBody.DefaultMecha(), McHead = MechaHead.DefaultMecha(), McLeg = MechaLeg.DefaultMecha() },
                new Operator { Name = "hoshino", ModelResourceUrl = "Hoshino", McBody = MechaBody.DefaultMecha(), McHead = MechaHead.DefaultMecha(), McLeg = MechaLeg.DefaultMecha() },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko", McBody = MechaBody.DefaultMecha(), McHead = MechaHead.DefaultMecha(), McLeg = MechaLeg.DefaultMecha() },
                new Operator { Name = "hoshino", ModelResourceUrl = "Hoshino", McBody = MechaBody.DefaultMecha(), McHead = MechaHead.DefaultMecha(), McLeg = MechaLeg.DefaultMecha() },
                new Operator { Name = "shiroko", ModelResourceUrl = "Shiroko", McBody = MechaBody.DefaultMecha(), McHead = MechaHead.DefaultMecha(), McLeg = MechaLeg.DefaultMecha() },
            };
        }

        public static List<MechaBase> GetMechas()
        {
            return new List<MechaBase>
            {
                MechaBody.DefaultMecha(),
                MechaHead.DefaultMecha(),
                MechaLeg.DefaultMecha()
            };
        }

        static List<string> clist = new List<string> { "Hoshino", "Shiroko"};
        public static Operator GetRandomOperate()
        {
            return new Operator
            {
                Name = "random test",
                ModelResourceUrl = clist[Random.Range(0, clist.Count)],
                McBody = MechaBody.DefaultMecha(),
                McHead = MechaHead.DefaultMecha(),
                McLeg = MechaLeg.DefaultMecha()
            };
        }
        public static List<Operator> GetRandomOperator(int t)
        {
            var ans = new List<Operator>(t);
            for(int i = 0; i < t; i++) ans.Add(GetRandomOperate());

            return ans;
        }
    }
}
