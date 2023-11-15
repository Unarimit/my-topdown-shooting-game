using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static MechaHead DefaultMecha()
        {
            return new MechaHead { IconUrl = "head", Name = "default head" , Critical = 5 , Accurate = 5 };
        }
        public override string ToString()
        {
            return $"ACC:\t{Accurate}%\n" +
                $"Crt:\t{Critical}%";
        }

    }
}
