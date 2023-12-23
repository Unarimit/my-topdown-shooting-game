using Assets.Scripts.Common;
using System.Text;
using Unity.VisualScripting;
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
    internal struct Produce
    {
        public string ItemId;
        public int Amount;
    }
    internal abstract class Building
    {

        /// <summary> 建筑Id </summary>
        public string BuildingId;

        /// <summary> 建筑名称 </summary>
        public string Name;

        /// <summary> 建筑描述 </summary>
        public string Description;

        /// <summary> 是否允许玩家建造 </summary>
        public bool CanBuild = true;

        /// <summary> 建筑放置格子 </summary>
        public Vector2Int Dimensions;

        /// <summary> 建筑模型 </summary>
        public string ModelUrl;

        /// <summary> 建筑类型 </summary>
        public BuildingType BuildingType;

        /// <summary> 建筑花费 </summary>
        public Produce[] Costs;

        public virtual string GetInfo()
        {
            var sb = new StringBuilder();
            sb.Append("消耗");
            foreach (var c in Costs)
            {
                sb.Append($"{ItemHelper.GetItem(c.ItemId).ItemName}:{c.Amount} ");
            }
            sb.AppendLine();
            sb.Append(Description);
            return sb.ToString();
        }
    }
}
