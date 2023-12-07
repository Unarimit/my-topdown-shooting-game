using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.EnviormentLogic
{
    /// <summary>
    /// 控制天空球自转
    /// </summary>
    internal class SkySphereController : MonoBehaviour
    {
        private void Update()
        {
            transform.Rotate(new Vector3(0, 1 * Time.deltaTime, 0));
        }
    }
}
