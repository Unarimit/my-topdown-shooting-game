using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Entities.Mechas
{
    [Serializable]
    public class MechaBody
    {
        public string IconUrl = "body";
        public string Name = "default body";
        public int HP = 5;
        public int HPRecover = 1;
    }
}
