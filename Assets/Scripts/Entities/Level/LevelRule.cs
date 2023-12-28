using Assets.Scripts.Services;
using Assets.Scripts.Services.Interface;
using System;

namespace Assets.Scripts.Entities.Level
{
    public class LevelRule
    {
        /// <summary> 关卡名称 </summary>
        public string LevelName;
        /// <summary> 关卡描述 </summary>
        public string Description;
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
    }
}
