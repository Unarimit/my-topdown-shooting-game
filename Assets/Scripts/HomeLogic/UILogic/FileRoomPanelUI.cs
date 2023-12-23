using Assets.Scripts.HomeLogic.Interface;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.HomeLogic.UILogic
{
    internal class FileRoomPanelUI : HomeUIBase, ISwitchUI
    {
        [SerializeField]
        Button m_returnBtn;
        private void Start()
        {
            m_returnBtn.onClick.AddListener(returnHome);
        }
        public void OnClick()
        {
            // do nothing
        }
        private void OnDestroy()
        {
            m_returnBtn.onClick.RemoveListener(returnHome);
        }
        private void returnHome()
        {
            _rootUI.SwitchPage(HomePage.MainView);
        }
    }
}
