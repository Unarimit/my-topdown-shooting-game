using Assets.Scripts.HomeLogic;
using System;

namespace Assets.Scripts.Entities.HomeMessage
{
    internal class HomeMessage
    {
        private static int _ID = 0;
        public int Id;
        public int Day;
        public Action<HomeContextManager> MessageAction;
        public HomeMessage()
        {
            Id = _ID++;
        }
        public void DoMessage(HomeContextManager context)
        {
            MessageAction(context);
        }
    }
}
