using Assets.Scripts.HomeLogic;
using System;

namespace Assets.Scripts.Entities.HomeMessage
{
    [Serializable]
    internal class HomeMessage
    {
        private static int _ID = 0;
        public int Id;
        public int Day;
        /// <summary> 用于序列化 </summary>
        public string MessageActionId;
        public Action<HomeContextManager> MessageAction { get; set; }
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
