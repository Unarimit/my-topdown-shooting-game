using Assets.Scripts.Entities.Buildings;
using Assets.Scripts.Entities.HomeMessage;
using Assets.Scripts.Entities.Level;
using Assets.Scripts.Entities.Mechas;
using Assets.Scripts.Services;
using Assets.Scripts.Services.Interface;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Entities.Save
{
    /// <summary>
    /// 存档数据类
    /// </summary>
    [Serializable]
    internal class SaveData
    {
        /// <summary> 存档id </summary>
        public string SaveId;

        public DateTime SaveTime;

        /// <summary> 玩家拥有的Op </summary>
        public IList<Operator> Operators;  // 没用MVC控制全局数据，改视图好麻烦。（想改成IDictionary来着）

        /// <summary> 玩家拥有的机甲 </summary>
        public IList<MechaBase> Mechas;

        /// <summary> 仓库（itemId, amount) </summary>
        public IDictionary<string, int> Inventory;

        /// <summary> 家园的建筑区域记录 </summary>
        public BuildingArea BuildingArea;

        /// <summary> 家园事件队列 </summary>
        public HomeMessageQueue HomeMessages;

        /// <summary> 根据这个判断是否需要结算建筑新一天产出 </summary>
        public bool OnNewDay;

        public SaveAbstract GenerateAbstract(string saveName)
        {
            return new SaveAbstract
            {
                SaveId = SaveId,
                SaveName = saveName,
                SaveTime = SaveTime,
                SaveDay = Inventory[MyConfig.ItemTable.GTime.ToString()],
                SaveDesc = "xxxxx", // 需要获取一些统计信息
                Resource = new Produce[] { 
                    new Produce { ItemId = MyConfig.ItemTable.Red.ToString(), Amount = Inventory[MyConfig.ItemTable.Red.ToString()] },
                    new Produce { ItemId = MyConfig.ItemTable.Iron.ToString(), Amount = Inventory[MyConfig.ItemTable.Iron.ToString()] },
                    new Produce { ItemId = MyConfig.ItemTable.Al.ToString(), Amount = Inventory[MyConfig.ItemTable.Al.ToString()] },
                    new Produce { ItemId = MyConfig.ItemTable.Ammo.ToString(), Amount = Inventory[MyConfig.ItemTable.Ammo.ToString()] },
                }
            };
        }
        public static SaveData GenerateSaveData(IGameDatabase database)
        {
            return new SaveData
            {
                SaveId = DateTime.Now.ToString(),
                SaveTime = DateTime.Now,
                Operators = database.Operators,
                Mechas = database.Mechas,
                Inventory = database.Inventory,
                BuildingArea = database.BuildingArea,
                HomeMessages = database.HomeMessages,
                OnNewDay = database.OnNewDay,

            };
        }
    }
}
