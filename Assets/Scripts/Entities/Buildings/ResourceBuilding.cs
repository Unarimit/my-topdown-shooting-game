
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
        public Produce[] Produces;
    }
}
