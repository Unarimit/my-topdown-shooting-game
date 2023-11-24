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

        public static MechaBody DefaultMecha()
        {
            return new MechaBody { IconUrl = "body", Name = "default body" , HP = 10 , HPRecover = 1 };
        }
        public override string ToString()
        {
            return $"HP:\t{HP}\n" +
                $"HPR:\t{HPRecover}";
        }
    }
}
