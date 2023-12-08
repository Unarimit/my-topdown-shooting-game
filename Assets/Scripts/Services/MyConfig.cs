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
        public enum DropoutTable
        {
            KillEnemy,
            KillTeam,
            Time,
            Key,
            Red, //测试道具1号
            Purple, //测试道具2号
            Sphere //测试道具3号
        }
        #endregion



    }
}
