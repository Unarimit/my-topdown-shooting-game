using Assets.Scripts.Common;
using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Buildings;
using Assets.Scripts.Entities.Mechas;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public static class MyConfig
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

        // 抽卡信息
        public static readonly List<Produce> SimpleCharacterCost =
            new() {
                new Produce { ItemId = ItemTable.Red.ToString(), Amount = 1 }
            };

        public static readonly List<Produce> ExpensiveCharacterCost =
            new List<Produce>() {
                new Produce { ItemId = ItemTable.Red.ToString(), Amount = 10 }
            };

        public static readonly List<Produce> SimpleMechaCost =
            new List<Produce>() {
                new Produce { ItemId = ItemTable.Iron.ToString(), Amount = 50 },
                new Produce { ItemId = ItemTable.Al.ToString(), Amount = 50 },
            };
        public static readonly List<Produce> ExpensiveMechaCost =
            new List<Produce>() {
                new Produce { ItemId = ItemTable.Iron.ToString(), Amount = 250 },
                new Produce { ItemId = ItemTable.Al.ToString(), Amount = 250 },
            };

        // name list
        public static readonly List<string> NameList = new()
        {
            "Sakura","Hikari","Aoi","Yumi","Ayumi","Haruka","Michiko","Kaori","Akari","Miku","Nana","Emi","Yui","Rei","Yuki","Asuka","Kiri","Chika","Nanami","Kumiko"
        };

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

        public enum Scene
        {
            Start,
            Home,
            Prepare,
            Playground,
        }

        #endregion



    }
}
