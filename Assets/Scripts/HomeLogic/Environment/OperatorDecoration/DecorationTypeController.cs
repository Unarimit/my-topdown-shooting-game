using System;
using UnityEngine;

namespace Assets.Scripts.HomeLogic.Environment.OperatorDecoration
{
    internal enum DecorationControllerType
    {
        Nothing,
        Walk,
        Talk
    }
    internal class DecorationTypeController : MonoBehaviour
    {
        /// <summary> 控制组使用的动画 </summary>
        public RuntimeAnimatorController m_AnimeController;

        public bool m_WithGun = false;

        [Tooltip("是否视为一个组一起加载")]
        public bool m_IsGroup = false;

        [Tooltip("用来批量定义行为")]
        public DecorationControllerType m_DecorationControllerType = DecorationControllerType.Nothing;
        [Tooltip("如果DecorationControllerType不为Nothing，这个字段也不要为空")]
        public string m_Param;
    }
}
