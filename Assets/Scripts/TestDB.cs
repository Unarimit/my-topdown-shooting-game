using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Mechas;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class TestDB
    {
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
    }
}
