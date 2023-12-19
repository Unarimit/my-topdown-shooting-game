
namespace Assets.Scripts.Entities.Buildings
{
    internal struct Produce
    {
        public string ItemId;
        public int Amount;
    }
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
    }
}
