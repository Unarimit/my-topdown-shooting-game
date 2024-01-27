using Assets.Scripts.Entities.HomeMessage;
using Assets.Scripts.Entities.Level;
using Assets.Scripts.Entities.Save;
using Assets.Scripts.Services.Interface;
using System;
using System.Collections.Generic;
using static Assets.Scripts.Services.MyConfig;

namespace Assets.Scripts.Services
{
    /// <summary>
    /// 常用的对游戏数据的方法
    /// </summary>
    internal class GameDataHelper
    {
        private IGameDatabase _database;
        public int DayNow { 
            get {
                return _database.Inventory[ItemTable.GTime.ToString()];
            } 
            set {
                _database.Inventory[ItemTable.GTime.ToString()] = value;
            } 
        }
        public GameDataHelper(IGameDatabase database)
        {
            _database = database;
        }

        public bool IsDay()
        {
            return DayNow % 2 == 0;
        }

        public bool IsInvation()
        {
            return DayNow % 7 == 0;
        }

        
        public void FinishLevel(EventLevelRule eventLevel)
        {
            // 0. 判断
            if(eventLevel.MessageAction is null)
            {
                throw new ArgumentNullException($"EventLevelRule named ${eventLevel.LevelName} do not have delegate action");
            }

            // 1. 推入队列
            _database.HomeMessages.Push(new HomeMessage { Day = DayNow + eventLevel.DelayDay, MessageAction = eventLevel.MessageAction });

            // last. 推进时间
            DayNow += 1; // 属性真方便啊
            _database.OnNewDay = true;
        }

        /// <summary>
        /// 结算战斗关卡
        /// </summary>
        /// <param name="result"></param>
        public void FinishLevel(CombatLevelResult result)
        {
            // 1. 战利品
            resourceAdd(result.Loot);
            // 2. 扣除体力
            foreach (var op in result.JoinOperator) op.Power -= 1;
            // 3. TODO: win loss?

            // last. 推进时间
            DayNow += 1;
            _database.OnNewDay = true;
        }

        private void resourceAdd(Dictionary<string, int> resource)
        {
            foreach (var x in resource)
            {
                if (_database.Inventory.ContainsKey(x.Key) is false)
                {
                    throw new ArgumentException($"GameDatabase do not have key named: {x.Key}");
                }

                _database.Inventory[x.Key] += x.Value;
            }
        }
    }
}
