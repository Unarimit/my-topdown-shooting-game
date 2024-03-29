﻿using Assets.Scripts.Entities;
using Assets.Scripts.Services;
using Assets.Scripts.Services.Database;

namespace Assets.Scripts
{
    /// <summary>
    /// 服务提供者（定位器）
    /// </summary>
    internal static class MyServices
    {
        /// <summary>
        /// 运行时存储，不等于存档
        /// </summary>
        public static IGameDatabase Database { get; }
        /// <summary>
        /// 游戏数据常用方法
        /// </summary>
        public static GameDataHelper GameDataHelper { get; }
        static MyServices()
        {
            // 测试使用
            //Database = new TestDatabase();
            Database = new FileDatabase();
            GameDataHelper = new GameDataHelper(Database);
        }
    }
}
