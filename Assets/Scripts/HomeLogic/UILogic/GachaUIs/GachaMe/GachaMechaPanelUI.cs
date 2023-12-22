using Assets.Scripts.Common;
using Assets.Scripts.HomeLogic.ContextExtend;
using Assets.Scripts.HomeLogic.Interface;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.HomeLogic.UILogic.GachaUIs.GachaMe
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

        [SerializeField]
        TextMeshProUGUI m_simpleNeedTMP;
        [SerializeField]
        TextMeshProUGUI m_expensiveNeedTMP;

        [SerializeField]
        GachaNewMachaInfoUI m_gachaNewMachaInfoUI;

        private void Start()
        {
            m_returnBtn.onClick.AddListener(returnHome);
            m_simpleGachaBtn.Slider = m_slider;
            m_expensiveGachaBtn.Slider = m_slider;
            m_simpleGachaBtn.OnHoldButtonFinishEvent += gachaSimple;
            m_expensiveGachaBtn.OnHoldButtonFinishEvent += gachaExpensive;

            m_simpleGachaBtn.OnHoldButtonPressEvent += gachaSimpleCheck;
            m_expensiveGachaBtn.OnHoldButtonPressEvent += gachaExpensiveCheck;

            m_simpleNeedTMP.text = _context.GetGachaNeed(GachaType.SimpleMecha);
            m_expensiveNeedTMP.text = _context.GetGachaNeed(GachaType.ExpensiveMecha);
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
            bool temp = _context.IsCanGacha(GachaType.SimpleMecha);
            if (temp is false)
            {
                TipsUI.GenerateNewTips(_context.GetGachaFailedTips(GachaType.SimpleMecha));
            }
            return temp;

        }
        private bool gachaExpensiveCheck()
        {
            bool temp = _context.IsCanGacha(GachaType.ExpensiveMecha);
            if (temp is false)
            {
                TipsUI.GenerateNewTips(_context.GetGachaFailedTips(GachaType.ExpensiveMecha));
            }
            return temp;
        }
        private void gachaSimple()
        {
            _context.DoGacha(GachaType.SimpleMecha);
            m_gachaNewMachaInfoUI.ShowMecha(MyServices.Database.Mechas[^1]);

        }
        private void gachaExpensive()
        {
            _context.DoGacha(GachaType.ExpensiveMecha);
            m_gachaNewMachaInfoUI.ShowMecha(MyServices.Database.Mechas[^1]);
        }

        public void OnClick()
        {
            // do nothing
        }
    }
}
