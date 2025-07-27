using Assets.Scripts.Entities;
using Assets.Scripts.Services;
using Assets.Scripts.Services.Database;
using Assets.Scripts.Services.Others;
using BehaviorDesigner.Runtime.Tasks;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
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
        public static IGameDatabase Database { get; private set; }
        /// <summary> Lua全局环境 </summary>
        public static LuaEnv LuaEnv { get; private set; }

        /// <summary> UI管理 </summary>
        public static IUiManager UiManager { get; private set; }

        /// <summary> 如果启动中有异步加载，可以用这个作为等待条件 </summary>
        public static TaskCompletionSource<bool> InitFin;


        #region 数据帮助类
        /// <summary> 游戏数据常用方法 </summary>
        public static GameDataHelper GameDataHelper { get; private set; }
        /// <summary> 背包访问常用方法 </summary>
        public static BagDataHelper BagDataHelper { get; private set; }
        public static OperatorDataHelper OpDataHelper { get; private set; }
        #endregion
        
        public static void LoadOnAppStart()
        {
            InitFin = new TaskCompletionSource<bool>();
            
            // 测试使用
            //Database = new TestDatabase();
            Database = new FileDatabase();
            LuaEnv = new LuaEnv();
            GameDataHelper = new GameDataHelper(Database);
            BagDataHelper = new BagDataHelper(Database);
            OpDataHelper = new OperatorDataHelper(Database); 
            
            InitFin.SetResult(true);
        }
    }
}
