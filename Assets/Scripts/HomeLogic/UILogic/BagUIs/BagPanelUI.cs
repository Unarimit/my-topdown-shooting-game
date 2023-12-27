using Assets.Scripts.Common.Interface;
using UnityEngine;

namespace Assets.Scripts.HomeLogic.UILogic.BagUIs
{
    internal class BagPanelUI : HomeUIBase, IOverlayUI
    {
        public override void Enter()
        {
            _rootUI.OverlayStack.Push(this);
            base.Enter();
        }

        private void Start()
        {
            transform.Find("ScrollView").GetComponent<BagScrollViewUI>().Inject(MyServices.Database.Inventory);
        }
        
    }
}
