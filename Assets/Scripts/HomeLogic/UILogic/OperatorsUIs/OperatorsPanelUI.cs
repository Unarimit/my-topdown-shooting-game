using Assets.Scripts.Common.Interface;
using Assets.Scripts.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.HomeLogic.UILogic.OperatorsUIs
{
    /// <summary>
    /// 家园中的展示角色信息面板，管理窗口的显示和消失
    /// </summary>
    internal class OperatorsPanelUI : HomeUIBase, IOverlayUI
    {
        public override void Enter()
        {
            _rootUI.OverlayStack.Push(this);
            base.Enter();
        }

        public override void Quit()
        {
            base.Quit();
        }
        private void Start()
        {
            transform.Find("ScrollView").GetComponent<OpScrollViewUI>().Inject(MyServices.Database.Operators);
        }
    }
}
