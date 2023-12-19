using UnityEngine;

namespace Assets.Scripts.Entities.Buildings
{
    internal enum BuildingType
    {
        /// <summary> 资源建筑 </summary>
        Resource,
        /// <summary> 战斗建筑 </summary>
        Combat
    }
    internal abstract class Building
    {
        /// <summary> 建筑名称 </summary>
        public string Name;

        /// <summary> 建筑描述 </summary>
        public string Description;

        /// <summary> 建筑放置格子 </summary>
        public Vector2Int Dimensions;

        /// <summary> 建筑模型 </summary>
        public string ModelUrl;

        /// <summary> 建筑类型 </summary>
        public BuildingType BuildingType;
    }
}
