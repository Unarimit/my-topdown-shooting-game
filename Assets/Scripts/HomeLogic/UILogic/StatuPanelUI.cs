using System;
using System.Linq;
using Assets.Scripts.Services;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.HomeLogic.UILogic
{
    internal class StatuPanelUI : HomeUIBase
    {

        [SerializeField]
        private TextMeshProUGUI m_timeTMP;
        [SerializeField]
        private TextMeshProUGUI m_populationTMP;
        [SerializeField]
        private TextMeshProUGUI m_electricTMP;
        [SerializeField]
        private TextMeshProUGUI m_ironTMP;
        [SerializeField]
        private TextMeshProUGUI m_ammoTMP;
        [SerializeField]
        private TextMeshProUGUI m_alTMP;
        [SerializeField]
        private TextMeshProUGUI m_gachaTMP;

        private void Start()
        {
            if(m_timeTMP != null) setTimeText();
            SetDataDisplay();
            MyServices.OpDataHelper.OnOpDataChange += SetDataDisplay;
        }

        private void OnDestroy()
        {
            MyServices.OpDataHelper.OnOpDataChange -= SetDataDisplay;
        }

        private void setTimeText()
        {
            var text = $"第{(_context.HomeVM.GTime / 2)}天，";
            if (_context.HomeVM.GTime % 2 == 0) text += "白天";
            else text += "夜晚";
            m_timeTMP.text = text;
        }
        private void SetDataDisplay()
        {
            m_populationTMP.text = MyServices.OpDataHelper.Operators.Count.ToString();
            m_electricTMP.text = MyServices.BagDataHelper.GetItemNum(MyConfig.ItemTable.Electric.ToString()).ToString();
            m_ironTMP.text = MyServices.BagDataHelper.GetItemNum(MyConfig.ItemTable.Iron.ToString()).ToString();
            m_ammoTMP.text = MyServices.BagDataHelper.GetItemNum(MyConfig.ItemTable.Ammo.ToString()).ToString();
            m_alTMP.text = MyServices.BagDataHelper.GetItemNum(MyConfig.ItemTable.Al.ToString()).ToString();
            m_gachaTMP.text = MyServices.BagDataHelper.GetItemNum(MyConfig.ItemTable.Red.ToString()).ToString();
        }
    }
}
