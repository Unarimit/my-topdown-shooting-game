using Assets.Scripts.Entities.Buildings;
using Assets.Scripts.HomeLogic.Interface;
using Assets.Scripts.HomeLogic.Placement;

namespace Assets.Scripts.HomeLogic.UILogic.BuildingUIs
{
    internal class BuildingUI : HomeUIBase, ISwitchUI
    {
        public void Inject(PlacementManager pm)
        {
            transform.Find("Scroll View").GetComponent<BuildingScrollViewUI>().Inject(MyServices.Database.Buildings, this);
        }

        public void OnSelect(Building building)
        {

        }
    }
}
