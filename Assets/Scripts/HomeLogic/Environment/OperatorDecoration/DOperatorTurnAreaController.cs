using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.HomeLogic.Environment.OperatorDecoration
{
    /// <summary>
    /// 装饰角色（散步）转向控制器
    /// </summary>
    internal class DOperatorTurnAreaController : MonoBehaviour
    {
        public Vector3 m_RotateAngle;
        public float m_RotateDuration;
        private void OnTriggerEnter(Collider other)
        {
            if(other.GetComponent<WalkDecorationOperator>() != null)
            {
                other.transform.DORotate(other.transform.eulerAngles + m_RotateAngle, m_RotateDuration);
            }
        }
    }
}
