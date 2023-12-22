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
        public float m_RotateSpeed = 1f;
        private void Update()
        {
            transform.Rotate(new Vector3(0, m_RotateSpeed * Time.deltaTime, 0));
        }
    }
}
