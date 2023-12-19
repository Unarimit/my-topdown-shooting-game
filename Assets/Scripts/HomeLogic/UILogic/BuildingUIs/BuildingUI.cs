using Assets.Scripts.Entities.Buildings;
using Assets.Scripts.HomeLogic.Interface;
using Assets.Scripts.HomeLogic.Placement;

namespace Assets.Scripts.HomeLogic.UILogic.BuildingUIs
{
    internal class BuildingUI : HomeUIBase, ISwitchUI
    {
        PlacementManager placementManager;
        public void Inject(PlacementManager pm)
        {
            placementManager = pm;
            transform.Find("Scroll View").GetComponent<BuildingScrollViewUI>().Inject(MyServices.Database.Buildings, this);
        }

        public void OnClick()
        {
            // DO Nothing
        }

        public void OnSelect(Building building)
        {
            placementManager.OnBuilding(building);
        }
    }
}
