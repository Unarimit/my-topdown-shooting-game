using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Entities.Mechas
{
    public enum MechaType
    {
        Head,
        Body,
        Leg
    }
    public class MechaBase
    {
        public string IconUrl;
        public string Name;

        public MechaType GetMechaType()
        {
            if (this.GetType() == typeof(MechaHead))
            {
                return MechaType.Head;
            }
            else if (this.GetType() == typeof(MechaBody))
            {
                return MechaType.Body;
            }
            else if (this.GetType() == typeof(MechaLeg))
            {
                return MechaType.Leg;
            }
            else
            {
                return MechaType.Body;
            }
        }
    }
}
