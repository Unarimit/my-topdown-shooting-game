using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Entities.Mechas
{
    [Serializable]
    public class MechaLeg
    {
        public string IconUrl = "leg";
        public string Name = "default leg";
        /// <summary>
        /// 等同于unity定义的speed
        /// </summary>
        public float Speed = 3f;
        /// <summary>
        /// 闪避率，百分比
        /// </summary>
        public int Dodge = 5;
    }
}
