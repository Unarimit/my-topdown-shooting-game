using Assets.Scripts.Entities.HomeMessage;
using Assets.Scripts.Entities.Level;
using Assets.Scripts.Services.Database;
using System;
using System.Collections.Generic;
using static Assets.Scripts.Services.MyConfig;

namespace Assets.Scripts.Services
{
    /// <summary>
    /// 常用的玩法状态判断方法
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

        /// <summary> 是否是白天 </summary>
        public bool IsDay()
        {
            return DayNow % 2 == 0;
        }

        /// <summary> 进行到哪一天 </summary>
        public int GetTime()
        {
            return _database.Inventory[MyConfig.ItemTable.GTime.ToString()];
        }

        /// <summary> 是否有入侵 </summary>
        public bool IsInvasion()
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
            _database.HomeMessages.Push(new HomeMessage { 
                Day = DayNow + eventLevel.DelayDay, 
                MessageAction = eventLevel.MessageAction, 
                MessageActionId = eventLevel.LevelId 
            });

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
            MyServices.BagDataHelper.ChangeItems(result.Loot);
            // 2. 扣除体力
            foreach (var op in result.JoinOperator) op.Power -= 1;
            // 3. TODO: win loss?

            // last. 推进时间
            DayNow += 1;
            _database.OnNewDay = true;
        }

    }
}
