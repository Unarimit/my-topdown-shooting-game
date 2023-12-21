using Assets.Scripts.HomeLogic.Interface;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.HomeLogic.UILogic.GachaUIs
{
    internal class GachaMechaPanelUI : HomeUIBase, ISwitchUI
    {
        [SerializeField]
        Button m_returnBtn;
        [SerializeField]
        MyHoldButton m_simpleGachaBtn;
        [SerializeField]
        MyHoldButton m_expensiveGachaBtn;
        [SerializeField]
        Slider m_slider;

        private void Start()
        {
            m_returnBtn.onClick.AddListener(returnHome);
            m_simpleGachaBtn.Slider = m_slider;
            m_expensiveGachaBtn.Slider = m_slider;
            m_simpleGachaBtn.OnHoldButtonFinishEvent += _context.GachaMacha;
            m_expensiveGachaBtn.OnHoldButtonFinishEvent += _context.GachaMacha;

        }
        private void OnDestroy()
        {
            m_returnBtn.onClick.RemoveListener(returnHome);
            m_simpleGachaBtn.OnHoldButtonFinishEvent -= _context.GachaMacha;
            m_expensiveGachaBtn.OnHoldButtonFinishEvent -= _context.GachaMacha;
        }

        private void returnHome()
        {
            _rootUI.SwitchPage(HomePage.CoreView);
        }


        public void OnClick()
        {
            // do nothing
        }
    }
}
