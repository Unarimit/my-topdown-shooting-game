using Assets.Scripts.Common;
using Assets.Scripts.HomeLogic.ContextExtend;
using Assets.Scripts.HomeLogic.Interface;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.HomeLogic.UILogic.GachaUIs
{
    internal class GachaCharacterPanelUI : HomeUIBase, ISwitchUI
    {
        [SerializeField]
        Button m_returnBtn;
        [SerializeField]
        MyHoldButton m_simpleGachaBtn;
        [SerializeField]
        MyHoldButton m_expensiveGachaBtn;
        [SerializeField]
        Slider m_slider;

        [SerializeField]
        TextMeshProUGUI m_simpleNeedTMP;
        [SerializeField]
        TextMeshProUGUI m_expensiveNeedTMP;

        private void Start()
        {
            m_returnBtn.onClick.AddListener(returnHome);
            m_simpleGachaBtn.Slider = m_slider;
            m_expensiveGachaBtn.Slider = m_slider;
            m_simpleGachaBtn.OnHoldButtonFinishEvent += gachaSimple;
            m_expensiveGachaBtn.OnHoldButtonFinishEvent += gachaExpensive;

            m_simpleGachaBtn.OnHoldButtonPressEvent += gachaSimpleCheck;
            m_expensiveGachaBtn.OnHoldButtonPressEvent += gachaExpensiveCheck;

            m_simpleNeedTMP.text = _context.GetGachaNeed(GachaType.SimpleCharacter);
            m_expensiveNeedTMP.text = _context.GetGachaNeed(GachaType.ExpensiveCharacter);
        }
        private void OnDestroy()
        {
            m_returnBtn.onClick.RemoveListener(returnHome);
            m_simpleGachaBtn.OnHoldButtonFinishEvent -= gachaSimple;
            m_expensiveGachaBtn.OnHoldButtonFinishEvent -= gachaExpensive;

            m_simpleGachaBtn.OnHoldButtonPressEvent -= gachaSimpleCheck;
            m_expensiveGachaBtn.OnHoldButtonPressEvent -= gachaExpensiveCheck;
        }

        private void returnHome()
        {
            _rootUI.SwitchPage(HomePage.CoreView);
        }
        private bool gachaSimpleCheck()
        {
            bool temp = _context.IsCanGacha(GachaType.SimpleCharacter);
            if (temp is false)
            {
                TipsUI.GenerateNewTips(_context.GetGachaFailedTips(GachaType.SimpleCharacter));
            }
            return temp;

        }
        private bool gachaExpensiveCheck()
        {
            bool temp = _context.IsCanGacha(GachaType.ExpensiveCharacter);
            if(temp is false)
            {
                TipsUI.GenerateNewTips(_context.GetGachaFailedTips(GachaType.ExpensiveCharacter));
            }
            return temp;
        }
        private void gachaSimple()
        {
            _context.DoGacha(GachaType.SimpleCharacter);

        }
        private void gachaExpensive()
        {
            _context.DoGacha(GachaType.ExpensiveCharacter);
        }

        public void OnClick()
        {
            // do nothing
        }
    }
}
