using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CombatLogic
{
    public interface IDelayWeaponController
    {

        /// <summary>
        /// 执行发射动作
        /// </summary>
        public void DoShootAction(Vector3 aim);

        /// <summary>
        /// 执行被延迟的动作
        /// </summary>
        public void DoDelayAction();


        /// <summary>
        /// 延时动作是否已经触发
        /// </summary>
        public bool IsTrigger();
    }
}
