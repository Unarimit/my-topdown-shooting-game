using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CombatLogic
{
    public class DelayWeapon
    {
        public Transform Caster { get; set; }
        /// <summary>
        /// 需要延迟的时间 秒
        /// </summary>
        public float DelayTime { get; set; }

        /// <summary>
        /// 延迟结算的时间 秒
        /// </summary>
        public float DelayEndTime { get; set; }

        public int Damage { get; set; }

        public float DamageRange { get; set; }

    }
}
