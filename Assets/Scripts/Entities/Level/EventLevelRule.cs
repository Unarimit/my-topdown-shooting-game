using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Entities.Level
{
    public class EventLevelRule : LevelRule
    {
        public EventLevelRule()
        {
            JumpScene = Services.MyConfig.Scene.Home;
        }
    }
}
