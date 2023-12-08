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
        public int Id;
        public string IconUrl;
        public string Name;
        public string Description;

        /// <summary>
        /// 无人使用时，这个值为空
        /// </summary>
        public Operator Operator { get; private set; }
        /// <summary>
        /// 应该只被数据库管理类调用
        /// </summary>
        public void DataBind(Operator op)
        {
            Operator = op;
        }

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
        public bool IsDefaultMecha()
        {
            return Id == -1;
        }
    }
}
