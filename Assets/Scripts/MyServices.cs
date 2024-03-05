using Assets.Scripts.Entities;
using Assets.Scripts.Services;
using Assets.Scripts.Services.Database;
using BehaviorDesigner.Runtime.Tasks;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using XLua;

namespace Assets.Scripts
{
    /// <summary>
    /// 服务提供者（定位器）
    /// </summary>
    internal static class MyServices
    {
        /// <summary> 运行时存储，不等于存档 </summary>
        public static IGameDatabase Database { get; }
        /// <summary> 游戏数据常用方法 </summary>
        public static GameDataHelper GameDataHelper { get; }
        /// <summary> Lua全局环境 </summary>
        public static LuaEnv LuaEnv { get; }
        static MyServices()
        {
            // 测试使用
            //Database = new TestDatabase();
            Database = new FileDatabase();
            GameDataHelper = new GameDataHelper(Database);
            LuaEnv = new LuaEnv();
        }
    }
}
