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

        public override string ToString()
        {
            return $"SPD:\t{Speed.ToString("0.00")}\n" +
            $"DOG:\t{Dodge}%";
        }
        public MechaLeg(string name, string iconUrl,  float speed, int dodge, int id = -1, string desc = "null")
        {
            Name = name;
            IconUrl = iconUrl;
            Speed = speed;
            Dodge = dodge;
            Id = id;
            Description = desc;
        }

        public static MechaLeg DefaultMecha()
        {
            return new MechaLeg (name: "Leg I", iconUrl: "leg",  speed:3f , dodge: 5, id: -1, desc:"default mecha" );
        }

        
    }
}
