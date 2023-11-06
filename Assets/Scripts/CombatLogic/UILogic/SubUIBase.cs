using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.UILogic
{
    public class SubUIBase : MonoBehaviour
    {
        public CombatContextManager _context => CombatContextManager.Instance;
        /// <summary>
        /// 实际上是SetActive
        /// </summary>
        /// <param name="isVisible"></param>
        public void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }
    }
}
