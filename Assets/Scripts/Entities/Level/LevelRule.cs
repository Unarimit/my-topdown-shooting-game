using Assets.Scripts.Services;
using Assets.Scripts.Services.Database;
using System;

namespace Assets.Scripts.Entities.Level
{
    public class LevelRule
    {
        /// <summary> 关卡名称 </summary>
        public string LevelName;
        /// <summary> 关卡描述 </summary>
        public string Description;
        /// <summary> 关卡图像 </summary>
        public string ImageUrl = null;
        /// <summary> 跳转页面 </summary>
        public MyConfig.Scene JumpScene;
        /// <summary> 触发条件 </summary>
        internal Func<IGameDatabase, bool> EnableFunc = DefaultEnableFunc;
        /// <summary> 是否独占 </summary>
        public bool IsOnly = false;

        internal static bool DefaultEnableFunc(IGameDatabase database)
        {
            return true;
        }
        /// <summary> 判断关卡是否可以激活 </summary> 
        internal bool IsActive => EnableFunc(MyServices.Database); // 这样写属性可以被重写，考虑委托的序列化问题
    }
}
