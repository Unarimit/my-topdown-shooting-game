
using System;
using UnityEngine;

namespace Assets.Scripts.Entities.Buildings
{
    [Serializable]
    internal struct PlaceInfo
    {
        /// <summary> 战斗区域的索引 </summary>
        public static int BattleIndex = 6;

        /// <summary> 放置建筑的id </summary>
        public string BuildingId;

        /// <summary> 放置平面的索引 </summary>
        public int AreaIndex;

        /// <summary> 放置位置 </summary>
        public Vector2Int PlacePosition { 
            get { 
                return new Vector2Int(_placePostionX, _placePostionY);
            }
            set
            {
                _placePostionX = value.x;
                _placePostionY = value.y;
            }
        }
        
        // 为了序列化
        private int _placePostionX;
        private int _placePostionY;

        /// <summary> 管理员Id, 用于序列化存储 </summary>
        public int AdminOperatorId;

        /// <summary> 管理员 </summary>
        public Operator AdminOperator{ get; set; }

    }
    [Serializable]
    internal class BuildingArea
    {
        /// <summary>  建筑的放置信息集合 </summary>
        public PlaceInfo[] PlaceInfos;
    }
}
