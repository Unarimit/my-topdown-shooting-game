using Assets.Scripts.HomeLogic.Interface;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.HomeLogic.UILogic.GachaUIs
{
    internal class GachaPanelUI : HomeUIBase, ISwitchUI
    {
        [SerializeField]
        Button m_getChaBtn;
        [SerializeField]
        Button m_getMaBtn;
        [SerializeField]
        Button m_returnBtn;

        private void Start()
        {
            m_getChaBtn.onClick.AddListener(onGetCharacter);
            m_getMaBtn.onClick.AddListener(onGetMacha);
            m_returnBtn.onClick.AddListener(returnHome);
        }
        private void OnDestroy()
        {
            m_getChaBtn.onClick.RemoveListener(onGetCharacter);
            m_getMaBtn.onClick.RemoveListener(onGetMacha);
            m_returnBtn.onClick.RemoveListener(returnHome);
        }
        public void OnClick()
        {
            // do nothing
        }

        private void onGetCharacter()
        {
            _rootUI.SwitchPage(HomePage.CoreCharacterView);
        }
        private void onGetMacha()
        {
            _rootUI.SwitchPage(HomePage.CoreMechaView);
        }
        private void returnHome()
        {
            _rootUI.SwitchPage(HomePage.MainView);
        }
    }
}
