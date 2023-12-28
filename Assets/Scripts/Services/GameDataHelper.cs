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
    }
}
