using Assets.Scripts.HomeLogic;
using System;

namespace Assets.Scripts.Entities.Level
{
    internal class EventLevelRule : LevelRule
    {
        public int DelayDay = 1;
        public Action<HomeContextManager> MessageAction;
        public EventLevelRule()
        {
            JumpScene = Services.MyConfig.Scene.Home;
        }
    }
}
