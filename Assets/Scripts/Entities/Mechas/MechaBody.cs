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
        public int HP = 5;
        public int HPRecover = 1;

        public static MechaBody DefaultMecha()
        {
            return new MechaBody { IconUrl = "body", Name = "default body" , HP = 5 , HPRecover = 1 };
        }
    }
}
