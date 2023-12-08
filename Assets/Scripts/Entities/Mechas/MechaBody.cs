using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Entities.Mechas
{
    [Serializable]
    public class MechaBody : MechaBase
    {
        public int HP;
        public int HPRecover;
        public MechaBody(string name, string iconUrl,  int hp, int hpRecover, int id = -1, string desc = "null")
        {
            IconUrl = iconUrl;
            Name = name;
            HP = hp;
            HPRecover = hpRecover;
            Id = id;
            Description = desc;
        }

        public static MechaBody DefaultMecha()
        {
            return new MechaBody ("Body I" , "body", 10, 1, -1, "default mecha");
        }
        public override string ToString()
        {
            return $"HP:\t{HP}\n" +
                $"HPR:\t{HPRecover}";
        }
    }
}
