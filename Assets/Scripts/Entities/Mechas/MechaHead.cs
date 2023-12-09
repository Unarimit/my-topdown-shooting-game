using System;

namespace Assets.Scripts.Entities.Mechas
{
    [Serializable]
    public class MechaHead : MechaBase
    {

        /// <summary>
        /// 暴击率，百分比
        /// </summary>
        public int Critical;
        /// <summary>
        /// 准确率，百分比
        /// </summary>
        public int Accurate;

        public MechaHead(string name, string iconUrl, int critical, int accurate, int id = -1, string desc = "null")
        {
            IconUrl = iconUrl;
            Name = name;
            Critical = critical;
            Accurate = accurate;
            Id = id;
            Description = desc;
        }

        public static MechaHead DefaultMecha()
        {
            return new MechaHead (name : "Head I" , iconUrl: "head", critical: 5 , accurate : 5, desc:"default mecha");
        }
        public override string ToString()
        {
            return  $"ACC:\t{Accurate}%\n" +
                $"CRT:\t{Critical}%";
        }

    }
}
