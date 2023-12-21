using Assets.Scripts.Entities.Buildings;
using Assets.Scripts.HomeLogic.Interface;
using Assets.Scripts.HomeLogic.Placement;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.HomeLogic.UILogic.BuildingUIs
{
    internal class BuildingUI : HomeUIBase, ISwitchUI
    {
        [SerializeField]
        Button m_cancelBtn; 
        [SerializeField]
        Button m_returnBtn;

        PlacementManager placementManager;
        public void Inject(PlacementManager pm)
        {
            placementManager = pm;
            transform.Find("Scroll View").GetComponent<BuildingScrollViewUI>().Inject(MyServices.Database.Buildings, this);
            m_cancelBtn.onClick.AddListener(deselect);
            m_returnBtn.onClick.AddListener(returnHome);
        }
        private void OnDestroy()
        {
            m_cancelBtn.onClick.RemoveListener(deselect);
            m_returnBtn.onClick.RemoveListener(returnHome);
        }
        public void OnClick()
        {
            // DO Nothing
        }

        public void OnSelect(Building building)
        {
            placementManager.OnBuilding(building);
            m_cancelBtn.interactable = building != null;
        }
        private void deselect()
        {
            OnSelect(null);
        }
        private void returnHome()
        {
            placementManager.OnBuilding(null);
            _rootUI.SwitchPage(HomePage.MainView);
        }
    }
}
