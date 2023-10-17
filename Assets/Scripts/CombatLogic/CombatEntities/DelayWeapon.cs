using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CombatLogic.CombatEntities
{
    public class DelayWeapon
    {
        /// <summary>
        /// 需要延迟的时间 秒
        /// </summary>
        public float DelayTime { get; set; }

        /// <summary>
        /// 延迟结算的时间 秒
        /// </summary>
        public float DelayEndTime { get; set; }

        public float Damage { get; set; }

        public float DamageRange { get; set; }

    }
}
