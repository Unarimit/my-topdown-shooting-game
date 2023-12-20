
using System;

namespace Assets.Scripts.Entities
{
    public enum GameItemType
    {
        /// <summary> 显示在Statu上的信息 </summary>
        Resources,
        /// <summary> 显示在仓库中的信息 </summary>
        Normal,
        /// <summary> 系统信息 </summary>
        System
    }
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
        /// 道具类型
        /// </summary>
        public GameItemType ItemType;

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
