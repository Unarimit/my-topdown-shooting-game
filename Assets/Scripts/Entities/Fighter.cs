using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Entities
{
    public enum FighterType
    {
        Bomber,
        AirFighter,
        
    }
    public static class FighterTypeExtension
    {
        public static string To2WordsString(this FighterType type)
        {
            if (type == FighterType.Bomber) return "BM";
            else if (type == FighterType.AirFighter) return "AF";
            else return "NL";
        }
    }
    public class Fighter
    {
        /// <summary>
        /// 战机类型
        /// </summary>
        public FighterType Type = FighterType.Bomber;
        /// <summary>
        /// 战机操作员，决定战机属性
        /// </summary>
        public Operator Operator;
    }
}
