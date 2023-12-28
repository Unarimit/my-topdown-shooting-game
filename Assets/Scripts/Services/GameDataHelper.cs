using Assets.Scripts.Entities.Level;
using Assets.Scripts.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Assets.Scripts.Services.MyConfig;

namespace Assets.Scripts.Services
{
    /// <summary>
    /// 常用的对游戏数据的方法
    /// </summary>
    internal class GameDataHelper
    {
        private IGameDatabase _database;
        public GameDataHelper(IGameDatabase database)
        {
            _database = database;
        }

        public bool IsDay()
        {
            return _database.Inventory[ItemTable.GTime.ToString()] % 2 == 0;
        }

        public bool IsInvation()
        {
            return _database.Inventory[ItemTable.GTime.ToString()] % 7 == 0;
        }

        /// <summary>
        /// 完成关卡，转移到下一个时间
        /// </summary>
        /// <param name="resourceAdd"></param>
        /// <exception cref="ArgumentException">仓库中缺少key</exception>
        public void FinishLevel(Dictionary<string, int> resourceAdd)
        {
            
            foreach(var x in resourceAdd)
            {
                if(_database.Inventory.ContainsKey(x.Key) is false)
                {
                    throw new ArgumentException($"GameDatabase do not have key named: {x.Key}");
                }

                _database.Inventory[x.Key] += x.Value;
            }
            // last. 推进时间
            _database.Inventory[ItemTable.GTime.ToString()] += 1;
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
            _database.Inventory[ItemTable.GTime.ToString()] += 1;
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
