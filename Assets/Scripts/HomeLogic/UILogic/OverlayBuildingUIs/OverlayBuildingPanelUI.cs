using Assets.Scripts.Common.Interface;
using UnityEngine;

namespace Assets.Scripts.HomeLogic.UILogic.OverlayBuildingUIs
{
    /// <summary>
    /// 
    /// </summary>
    internal class OverlayBuildingPanelUI : HomeUIBase, IOverlayUI
    {
        public override void Enter()
        {
            _rootUI.OverlayStack.Push(this);
            base.Enter();
        }
        private void OnEnable()
        {
            transform.Find("ScrollView").GetComponent<ObScrollViewUI>()
                .Inject(MyServices.Database.BuildingArea.PlaceInfos, MyServices.Database.Buildings);
        }
    }
}
