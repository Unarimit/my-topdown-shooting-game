using Assets.Scripts.Common;
using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Mechas;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Services
{
    internal static class MyConfig
    {
        #region 常量配置
        public static readonly int CHARACTER_LAYER = LayerMask.NameToLayer("Character");
        public static readonly int DOBJECT_LAYER = LayerMask.NameToLayer("DestructibleObject");
        public static readonly string PLAYER_TAG = "Player";
        public static readonly string UNTAGED_TAG = "Untagged";
        public static readonly Color TeamColor = new Color32(124, 208, 255, 255);
        public static readonly Color EnemyColor = new Color32(255, 100, 100, 255);
        public static readonly Color PlayerColor = new Color32(248, 255, 13, 255);
        public enum SkillSelectorStr
        {
            Trigger
        }
        #endregion

        #region 全局信息（如关卡、仓库状态）
        /// <summary> 掉落物表，用于ToString，防止拼错 </summary>
        public enum ItemTable
        {
            // 战斗系统
            KillEnemy, // 敌人死亡标志
            KillTeam, // 队友死亡标志
            Time, // 战斗时间
            Key,  // 测试互动道具

            // 仓库资源
            Electric, // 电
            Iron, // 铁
            Ammo, // 弹药
            Al, // 铝
            Red, // 测试道具1号，充当抽卡道具

            Purple, //测试道具2号
            Sphere, //测试道具3号

            // 家园系统
            GTime, // 全局“时间”
            PowerRecover,
        }
        #endregion



    }
}
