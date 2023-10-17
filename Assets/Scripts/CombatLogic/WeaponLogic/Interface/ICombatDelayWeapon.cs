using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CombatLogic
{
    public interface ICombatDelayWeapon
    {
        /// <summary>
        /// 执行被延迟的动作
        /// </summary>
        public void DoDelayAction();
    }
}
