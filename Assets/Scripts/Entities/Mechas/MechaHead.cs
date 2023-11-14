using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Entities.Mechas
{
    [Serializable]
    public class MechaHead
    {

        public string IconUrl = "head";
        public string Name = "default head";
        /// <summary>
        /// 暴击率，百分比
        /// </summary>
        public int Critical = 5;
        /// <summary>
        /// 准确率，百分比
        /// </summary>
        public int Accurate = 5;

    }
}
