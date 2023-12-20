
using Assets.Scripts.Common;
using System.Text;

namespace Assets.Scripts.Entities.Buildings
{
    internal class ResourceBuilding : Building
    {
        public ResourceBuilding()
        {
            BuildingType = BuildingType.Resource;
        }
        /// <summary>
        /// 生产物，每半天（代码逻辑的+1）结算一次
        /// </summary>
        public Produce[] Produces;

        public override string GetInfo()
        {
            var sb = new StringBuilder();
            sb.Append(base.GetInfo());
            sb.AppendLine();
            sb.Append("产出");
            foreach (var p in Produces)
            {
                sb.Append($"{ItemHelper.GetItem(p.ItemId).ItemName}:{p.Amount} ");
            }
            return sb.ToString();
        }
    }
}
