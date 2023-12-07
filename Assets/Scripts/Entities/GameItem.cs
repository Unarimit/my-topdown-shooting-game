
using System;

namespace Assets.Scripts.Entities
{
    // 游戏中的道具
    [Serializable]
    public class GameItem
    {
        /// <summary>
        /// 道具Id
        /// </summary>
        public string ItemId;

        /// <summary>
        /// 道具名称
        /// </summary>
        public string ItemName;

        /// <summary>
        /// 道具描述
        /// </summary>
        public string Description;

        /// <summary>
        /// 图标资源
        /// </summary>
        public string IconUrl;

        /// <summary>
        /// 是否允许显示（仓库，掉落物UI等）
        /// </summary>
        public bool IsDisplay;
    }
}
