using Assets.Scripts.HomeLogic;
using System;

namespace Assets.Scripts.Entities.HomeMessage
{
    [Serializable]
    public class HomeMessage
    {
        private static int _ID = 0;
        [NonSerialized]
        /// <summary> 临时Id，不应序列化存储 </summary>
        public int Id;
        public int Day;
        /// <summary> 用于序列化 </summary>
        public string MessageActionId;
        internal Action<HomeContextManager> MessageAction { get; set; }
        public HomeMessage()
        {
            Id = _ID++;
        }
        internal void DoMessage(HomeContextManager context)
        {
            MessageAction(context);
        }
    }
}
