using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Entities.Mechas
{
    [Serializable]
    public class MechaLeg : MechaBase
    {
        /// <summary>
        /// 等同于unity定义的speed
        /// </summary>
        public float Speed;
        /// <summary>
        /// 闪避率，百分比
        /// </summary>
        public int Dodge;

        public static MechaLeg DefaultMecha()
        {
            return new MechaLeg { IconUrl = "leg" , Name = "default leg" , Speed = 3f , Dodge = 5 };
        }
    }
}
