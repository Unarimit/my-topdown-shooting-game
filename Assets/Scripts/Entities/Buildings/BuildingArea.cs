
using UnityEngine;

namespace Assets.Scripts.Entities.Buildings
{
    internal struct PlaceInfo
    {
        /// <summary>
        /// 战斗区域的索引
        /// </summary>
        public static int BattleIndex = 6;

        /// <summary> 放置建筑的id </summary>
        public string BuildingId;

        /// <summary> 放置平面的索引 </summary>
        public int AreaIndex;

        /// <summary> 放置位置 </summary>
        public Vector2Int PlacePosition;
    }
    internal class BuildingArea
    {
        /// <summary>  建筑的放置信息集合 </summary>
        public PlaceInfo[] PlaceInfos;
    }
}
